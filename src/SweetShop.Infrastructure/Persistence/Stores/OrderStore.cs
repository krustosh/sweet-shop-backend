using Microsoft.EntityFrameworkCore;
using SweetShop.Application.Features.Orders;
using SweetShop.Domain.Entities;
using SweetShop.Domain.Enums;
using SweetShop.Infrastructure.Persistence.Context;

namespace SweetShop.Infrastructure.Persistence.Stores;

/// <summary>
/// Provides persistence operations required by customer order management.
/// </summary>
public sealed class OrderStore : IOrderStore
{
    private readonly SweetShopDbContext dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderStore"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public OrderStore(SweetShopDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        this.dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Order>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        ValidateId(
            customerId,
            nameof(customerId),
            "Customer ID is required.");

        return await dbContext.Set<Order>()
            .AsNoTracking()
            .Include(order => order.Items)
            .Where(order => order.CustomerId == customerId)
            .OrderByDescending(order => order.CreatedAt)
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Order?> GetByCustomerAndIdAsync(
        Guid customerId,
        Guid orderId,
        CancellationToken cancellationToken)
    {
        ValidateId(
            customerId,
            nameof(customerId),
            "Customer ID is required.");

        ValidateId(
            orderId,
            nameof(orderId),
            "Order ID is required.");

        return await dbContext.Set<Order>()
            .Include(order => order.Items)
            .SingleOrDefaultAsync(
                order =>
                    order.Id == orderId &&
                    order.CustomerId == customerId,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Cart?> GetCartByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        ValidateId(
            customerId,
            nameof(customerId),
            "Customer ID is required.");

        return await dbContext.Set<Cart>()
            .Include(cart => cart.Items)
            .SingleOrDefaultAsync(
                cart => cart.CustomerId == customerId,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<OrderItemDetails?> GetActiveVariantDetailsAsync(
        Guid shopId,
        Guid productVariantId,
        CancellationToken cancellationToken)
    {
        ValidateId(
            shopId,
            nameof(shopId),
            "Shop ID is required.");

        ValidateId(
            productVariantId,
            nameof(productVariantId),
            "Product variant ID is required.");

        return await (
            from variant in dbContext.Set<ProductVariant>()
            join product in dbContext.Set<Product>()
                on variant.ProductId equals product.Id
            where variant.Id == productVariantId
                && product.ShopId == shopId
                && variant.Status == ProductVariantStatus.Active
                && product.Status == ProductStatus.Active
            select new OrderItemDetails(
                variant.Id,
                product.Id,
                product.Name,
                variant.Name,
                variant.Quantity.Unit,
                variant.Price.Amount,
                variant.Price.Currency))
            .SingleOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Inventory?> GetInventoryByVariantIdAsync(
        Guid shopId,
        Guid productVariantId,
        CancellationToken cancellationToken)
    {
        ValidateId(
            shopId,
            nameof(shopId),
            "Shop ID is required.");

        ValidateId(
            productVariantId,
            nameof(productVariantId),
            "Product variant ID is required.");

        return await (
            from inventory in dbContext.Set<Inventory>()
            join variant in dbContext.Set<ProductVariant>()
                on inventory.ProductVariantId equals variant.Id
            join product in dbContext.Set<Product>()
                on variant.ProductId equals product.Id
            where inventory.ProductVariantId == productVariantId
                && product.ShopId == shopId
            select inventory)
            .SingleOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Address?> GetAddressByIdAsync(
        Guid addressId,
        CancellationToken cancellationToken)
    {
        ValidateId(
            addressId,
            nameof(addressId),
            "Address ID is required.");

        return await dbContext.Set<Address>()
            .SingleOrDefaultAsync(
                address => address.Id == addressId,
                cancellationToken);
    }

    /// <inheritdoc />
    public void Add(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        dbContext.Set<Order>().Add(order);
    }

    /// <inheritdoc />
    public void AddItem(OrderItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        dbContext.Set<OrderItem>().Add(item);
    }

    /// <inheritdoc />
    public void AddInventoryTransaction(
        InventoryTransaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        dbContext.Set<InventoryTransaction>().Add(transaction);
    }

    private static void ValidateId(
        Guid id,
        string parameterName,
        string message)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                message,
                parameterName);
        }
    }
}