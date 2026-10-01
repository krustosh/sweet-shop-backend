using CartEntity = SweetShop.Domain.Entities.Cart;
using InventoryEntity = SweetShop.Domain.Entities.Inventory;
using SweetShop.Application.Features.Orders.Requests;
using SweetShop.Application.Features.Orders.Responses;
using SweetShop.Application.Interfaces;
using SweetShop.Domain.Common;
using SweetShop.Domain.Entities;
using SweetShop.Domain.Enums;
using SweetShop.Domain.ValueObjects;
using SweetShop.Application.Features.Customers;

namespace SweetShop.Application.Features.Orders;

/// <summary>
/// Provides customer-facing order management operations.
/// </summary>
public sealed class OrderService : IOrderService
{
    private readonly ICurrentUser currentUser;
    private readonly ICustomerStore customerStore;
    private readonly IOrderStore orderStore;
    private readonly IShopContext shopContext;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderService"/> class.
    /// </summary>
    /// <param name="currentUser">The current authenticated user context.</param>
    /// <param name="customerStore">The customer store.</param>
    /// <param name="orderStore">The order store.</param>
    /// <param name="shopContext">The current shop context.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public OrderService(
        ICurrentUser currentUser,
        ICustomerStore customerStore,
        IOrderStore orderStore,
        IShopContext shopContext,
        IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(currentUser);
        ArgumentNullException.ThrowIfNull(customerStore);
        ArgumentNullException.ThrowIfNull(orderStore);
        ArgumentNullException.ThrowIfNull(shopContext);
        ArgumentNullException.ThrowIfNull(unitOfWork);

        this.currentUser = currentUser;
        this.customerStore = customerStore;
        this.orderStore = orderStore;
        this.shopContext = shopContext;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<OrderResponse>> GetMyOrdersAsync(
        CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(
            cancellationToken);

        var orders = await orderStore.GetByCustomerIdAsync(
            customer.Id,
            cancellationToken);

        return orders
            .Select(Map)
            .ToArray();
    }

    /// <inheritdoc />
    public async Task<OrderResponse?> GetMyOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        ValidateId(orderId, nameof(orderId));

        var customer = await GetCurrentCustomerAsync(
            cancellationToken);

        var order = await orderStore.GetByCustomerAndIdAsync(
            customer.Id,
            orderId,
            cancellationToken);

        return order is null
            ? null
            : Map(order);
    }

    /// <inheritdoc />
    public async Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var customer = await GetCurrentCustomerAsync(
            cancellationToken);

        var cart = await orderStore.GetCartByCustomerIdAsync(
            customer.Id,
            cancellationToken);

        if (cart is null || cart.Items.Count == 0)
        {
            throw new InvalidOperationException(
                "The cart must contain at least one item before an order can be created.");
        }

        var addressSnapshot =
            await GetDeliveryAddressSnapshotAsync(
                customer.Id,
                request,
                cancellationToken);

        var itemDetails =
            new List<(
                CartItem CartItem,
                OrderItemDetails Details,
                InventoryEntity Inventory)>();

        foreach (var cartItem in cart.Items)
        {
            var details =
                await orderStore.GetActiveVariantDetailsAsync(
                    shopContext.ShopId,
                    cartItem.ProductVariantId,
                    cancellationToken);

            if (details is null)
            {
                throw new InvalidOperationException(
                    "One or more products in the cart are no longer available.");
            }

            var inventory =
                await orderStore.GetInventoryByVariantIdAsync(
                    shopContext.ShopId,
                    cartItem.ProductVariantId,
                    cancellationToken);

            if (inventory is null)
            {
                throw new InvalidOperationException(
                    $"Inventory is not configured for product variant '{cartItem.ProductVariantId}'.");
            }

            if (cartItem.Quantity > inventory.AvailableQuantity)
            {
                throw new InvalidOperationException(
                    $"Insufficient inventory for '{details.ProductName} - {details.VariantName}'.");
            }

            itemDetails.Add((
                cartItem,
                details,
                inventory));
        }

        var deliveryFee = new Money(
            0,
            DomainConstants.CurrencyInr);

        var discountAmount = new Money(
            0,
            DomainConstants.CurrencyInr);

        var order = new Order(
            GenerateOrderNumber(),
            customer.Id,
            request.FulfillmentType,
            deliveryFee,
            discountAmount,
            request.CustomerNote,
            addressSnapshot,
            request.FulfillmentType == FulfillmentType.Pickup
                ? "Customer pickup"
                : null);

        orderStore.Add(order);

        foreach (var item in itemDetails)
        {
            var unitPrice = new Money(
                item.Details.PriceAmount,
                item.Details.Currency);

            var orderItem = new OrderItem(
                order.Id,
                item.Details.ProductId,
                item.Details.ProductVariantId,
                item.Details.ProductName,
                item.Details.VariantName,
                item.CartItem.Quantity,
                item.Details.Unit,
                unitPrice);

            order.AddItem(orderItem);
            orderStore.AddItem(orderItem);

            var previousQuantity =
                item.Inventory.AvailableQuantity;

            item.Inventory.Reserve(
                item.CartItem.Quantity);

            var transaction = new InventoryTransaction(
                item.Inventory.Id,
                InventoryTransactionType.Order,
                -item.CartItem.Quantity,
                previousQuantity,
                item.Inventory.AvailableQuantity,
                GetCurrentUserId(),
                "ORDER_RESERVATION",
                order.Id,
                "Inventory reserved for order.");

            orderStore.AddInventoryTransaction(
                transaction);
        }

        cart.Clear();

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Map(order);
    }

    /// <inheritdoc />
    public async Task<OrderResponse?> CancelAsync(
        Guid orderId,
        CancelOrderRequest request,
        CancellationToken cancellationToken)
    {
        ValidateId(orderId, nameof(orderId));
        ArgumentNullException.ThrowIfNull(request);

        var customer = await GetCurrentCustomerAsync(
            cancellationToken);

        var order = await orderStore.GetByCustomerAndIdAsync(
            customer.Id,
            orderId,
            cancellationToken);

        if (order is null)
        {
            return null;
        }

        if (order.Status is
            OrderStatus.OutForDelivery or
            OrderStatus.Delivered or
            OrderStatus.PickedUp or
            OrderStatus.Completed)
        {
            throw new InvalidOperationException(
                "The order can no longer be cancelled.");
        }

        foreach (var orderItem in order.Items)
        {
            var inventory =
                await orderStore.GetInventoryByVariantIdAsync(
                    shopContext.ShopId,
                    orderItem.ProductVariantId,
                    cancellationToken);

            if (inventory is null)
            {
                throw new InvalidOperationException(
                    "Inventory could not be found while cancelling the order.");
            }

            var previousQuantity =
                inventory.AvailableQuantity;

            inventory.ReleaseReservation(
                orderItem.Quantity);

            var transaction = new InventoryTransaction(
                inventory.Id,
                InventoryTransactionType.Order,
                orderItem.Quantity,
                previousQuantity,
                inventory.AvailableQuantity,
                GetCurrentUserId(),
                "ORDER_CANCELLATION",
                order.Id,
                request.Reason);

            orderStore.AddInventoryTransaction(
                transaction);
        }

        order.Cancel(request.Reason);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Map(order);
    }

    private async Task<Customer> GetCurrentCustomerAsync(
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var customer = await customerStore.GetByUserIdAsync(
            userId,
            cancellationToken);

        if (customer is null)
        {
            throw new InvalidOperationException(
                "Authenticated user is not associated with a customer.");
        }

        return customer;
    }

    private async Task<DeliveryAddressSnapshot?>
        GetDeliveryAddressSnapshotAsync(
            Guid customerId,
            CreateOrderRequest request,
            CancellationToken cancellationToken)
    {
        if (request.FulfillmentType == FulfillmentType.Pickup)
        {
            return null;
        }

        if (!request.DeliveryAddressId.HasValue)
        {
            throw new InvalidOperationException(
                "A delivery address is required for delivery orders.");
        }

        var address = await orderStore.GetAddressByIdAsync(
            request.DeliveryAddressId.Value,
            cancellationToken);

        if (address is null ||
            address.CustomerId != customerId)
        {
            throw new InvalidOperationException(
                "The specified delivery address was not found.");
        }

        return new DeliveryAddressSnapshot(
            address.RecipientName,
            address.MobileNumber,
            address.AddressLine1,
            address.AddressLine2,
            address.Landmark,
            address.Area,
            address.City,
            address.State,
            address.PostalCode,
            address.Latitude,
            address.Longitude);
    }

    private Guid GetCurrentUserId()
    {
        if (!currentUser.UserId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "Authenticated user ID was not found.");
        }

        return currentUser.UserId.Value;
    }

    private static string GenerateOrderNumber()
    {
        var suffix = Guid.NewGuid()
            .ToString("N")[..8];

        return $"SS-{DateTime.UtcNow:yyyyMMddHHmmss}-{suffix}";
    }

    private static OrderResponse Map(Order order)
    {
        var items = order.Items
            .Select(item =>
                new OrderItemResponse(
                    item.Id,
                    item.ProductId,
                    item.ProductVariantId,
                    item.ProductName,
                    item.VariantName,
                    item.Quantity,
                    item.Unit,
                    item.UnitPrice.Amount,
                    item.LineTotal.Amount,
                    item.UnitPrice.Currency,
                    item.CreatedAt))
            .ToArray();

        var address = order.DeliveryAddressSnapshot is null
            ? null
            : new OrderAddressResponse(
                order.DeliveryAddressSnapshot.RecipientName,
                order.DeliveryAddressSnapshot.MobileNumber,
                order.DeliveryAddressSnapshot.AddressLine1,
                order.DeliveryAddressSnapshot.AddressLine2,
                order.DeliveryAddressSnapshot.Landmark,
                order.DeliveryAddressSnapshot.Area,
                order.DeliveryAddressSnapshot.City,
                order.DeliveryAddressSnapshot.State,
                order.DeliveryAddressSnapshot.PostalCode,
                order.DeliveryAddressSnapshot.Latitude,
                order.DeliveryAddressSnapshot.Longitude);

        return new OrderResponse(
            order.Id,
            order.OrderNumber,
            order.CustomerId,
            order.FulfillmentType,
            order.Status,
            order.PaymentStatus,
            order.Subtotal.Amount,
            order.DeliveryFee.Amount,
            order.DiscountAmount.Amount,
            order.TotalAmount.Amount,
            order.TotalAmount.Currency,
            order.CustomerNote,
            address,
            order.PickupDetails,
            items,
            order.PlacedAt,
            order.AcceptedAt,
            order.PreparingAt,
            order.ReadyAt,
            order.CompletedAt,
            order.CancelledAt,
            order.CancellationReason,
            order.CreatedAt,
            order.UpdatedAt);
    }

    private static void ValidateId(
        Guid id,
        string parameterName)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "Order ID is required.",
                parameterName);
        }
    }
}