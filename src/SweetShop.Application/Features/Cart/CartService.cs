using SweetShop.Application.Features.Cart.Requests;
using SweetShop.Application.Features.Cart.Responses;
using SweetShop.Application.Features.Customers;
using SweetShop.Application.Interfaces;
using SweetShop.Domain.Entities;
using CartEntity = SweetShop.Domain.Entities.Cart;

namespace SweetShop.Application.Features.Cart;

/// <summary>
/// Provides business operations for the authenticated customer's shopping cart.
/// </summary>
public sealed class CartService : ICartService
{
    private readonly ICurrentUser currentUser;
    private readonly ICustomerStore customerStore;
    private readonly ICartStore cartStore;
    private readonly IShopContext shopContext;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="CartService"/> class.
    /// </summary>
    /// <param name="currentUser">The current authenticated user context.</param>
    /// <param name="customerStore">The customer persistence store.</param>
    /// <param name="cartStore">The cart persistence store.</param>
    /// <param name="shopContext">The current shop context.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public CartService(
        ICurrentUser currentUser,
        ICustomerStore customerStore,
        ICartStore cartStore,
        IShopContext shopContext,
        IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(currentUser);
        ArgumentNullException.ThrowIfNull(customerStore);
        ArgumentNullException.ThrowIfNull(cartStore);
        ArgumentNullException.ThrowIfNull(shopContext);
        ArgumentNullException.ThrowIfNull(unitOfWork);

        this.currentUser = currentUser;
        this.customerStore = customerStore;
        this.cartStore = cartStore;
        this.shopContext = shopContext;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<CartResponse> GetMyCartAsync(
        CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);

        var cart = await GetOrCreateCartAsync(
            customer.Id,
            cancellationToken);

        return await BuildResponseAsync(
            cart,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CartResponse> AddItemAsync(
        AddCartItemRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var customer = await GetCurrentCustomerAsync(
            cancellationToken);

        var variant =
            await cartStore.GetActiveVariantDetailsAsync(
                shopContext.ShopId,
                request.ProductVariantId,
                cancellationToken);

        if (variant is null)
        {
            throw new InvalidOperationException(
                "The specified product variant is not available.");
        }

        var cart = await GetOrCreateCartAsync(
            customer.Id,
            cancellationToken);

        var existingItem = cart.Items.SingleOrDefault(
            item =>
                item.ProductVariantId ==
                request.ProductVariantId);

        if (existingItem is null)
        {
            var newItem = new CartItem(
                cart.Id,
                request.ProductVariantId,
                request.Quantity);

            cart.AddItem(newItem);
            cartStore.AddItem(newItem);
        }
        else
        {
            existingItem.ChangeQuantity(
                existingItem.Quantity +
                request.Quantity);
        }

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        return await BuildResponseAsync(
            cart,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CartResponse?> UpdateItemAsync(
        Guid cartItemId,
        UpdateCartItemRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var customer = await GetCurrentCustomerAsync(
            cancellationToken);

        var cart = await cartStore.GetByCustomerIdAsync(
            customer.Id,
            cancellationToken);

        if (cart is null)
        {
            return null;
        }

        var item = cart.Items.SingleOrDefault(
            cartItem => cartItem.Id == cartItemId);

        if (item is null)
        {
            return null;
        }

        var variant =
            await cartStore.GetActiveVariantDetailsAsync(
                shopContext.ShopId,
                item.ProductVariantId,
                cancellationToken);

        if (variant is null)
        {
            throw new InvalidOperationException(
                "The product variant in the cart is no longer available.");
        }

        item.ChangeQuantity(request.Quantity);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        return await BuildResponseAsync(
            cart,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveItemAsync(
        Guid cartItemId,
        CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(
            cancellationToken);

        var cart = await cartStore.GetByCustomerIdAsync(
            customer.Id,
            cancellationToken);

        if (cart is null)
        {
            return false;
        }

        var item = cart.Items.SingleOrDefault(
            cartItem => cartItem.Id == cartItemId);

        if (item is null)
        {
            return false;
        }

        cart.RemoveItem(cartItemId);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> ClearAsync(
        CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(
            cancellationToken);

        var cart = await cartStore.GetByCustomerIdAsync(
            customer.Id,
            cancellationToken);

        if (cart is null)
        {
            return false;
        }

        cart.Clear();

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    private async Task<Customer> GetCurrentCustomerAsync(
        CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "Authenticated user ID was not found.");
        }

        var customer =
            await customerStore.GetByUserIdAsync(
                userId.Value,
                cancellationToken);

        if (customer is null)
        {
            throw new InvalidOperationException(
                "Customer profile was not found.");
        }

        return customer;
    }

    private async Task<CartEntity> GetOrCreateCartAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        var cart =
            await cartStore.GetByCustomerIdAsync(
                customerId,
                cancellationToken);

        if (cart is not null)
        {
            return cart;
        }

        cart = new CartEntity(customerId);

        cartStore.Add(cart);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        return cart;
    }

    private async Task<CartResponse> BuildResponseAsync(
        CartEntity cart,
        CancellationToken cancellationToken)
    {
        var items = new List<CartItemResponse>(
            cart.Items.Count);

        foreach (var item in cart.Items.OrderBy(
                     item => item.CreatedAt))
        {
            var variant =
                await cartStore.GetActiveVariantDetailsAsync(
                    shopContext.ShopId,
                    item.ProductVariantId,
                    cancellationToken);

            if (variant is null)
            {
                throw new InvalidOperationException(
                    "The cart contains a product variant that is no longer available.");
            }

            var lineTotal = decimal.Round(
                variant.PriceAmount * item.Quantity,
                2,
                MidpointRounding.AwayFromZero);

            items.Add(
                new CartItemResponse(
                    item.Id,
                    item.ProductVariantId,
                    variant.ProductId,
                    variant.ProductName,
                    variant.VariantName,
                    variant.VariantQuantity,
                    variant.Unit,
                    item.Quantity,
                    variant.PriceAmount,
                    lineTotal,
                    variant.Currency,
                    item.CreatedAt,
                    item.UpdatedAt));
        }

        var subtotal = decimal.Round(
            items.Sum(item => item.LineTotal),
            2,
            MidpointRounding.AwayFromZero);

        var currency =
            items
                .Select(item => item.Currency)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .SingleOrDefault()
            ?? SweetShop.Domain.Common.DomainConstants.CurrencyInr;

        return new CartResponse(
            cart.Id,
            cart.CustomerId,
            items,
            items.Count,
            items.Sum(item => item.Quantity),
            subtotal,
            currency,
            cart.CreatedAt,
            cart.UpdatedAt);
    }
}