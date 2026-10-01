using Microsoft.EntityFrameworkCore;
using SweetShop.Application.Features.Cart;
using SweetShop.Domain.Entities;
using SweetShop.Domain.Enums;
using SweetShop.Infrastructure.Persistence.Context;

namespace SweetShop.Infrastructure.Persistence.Stores;

/// <summary>
/// Provides persistence operations for customer shopping carts.
/// </summary>
public sealed class CartStore : ICartStore
{
    private readonly SweetShopDbContext dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="CartStore"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public CartStore(SweetShopDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        this.dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<Cart?> GetByCustomerIdAsync(
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
    public async Task<CartItem?> GetItemByIdAsync(
        Guid cartId,
        Guid cartItemId,
        CancellationToken cancellationToken)
    {
        ValidateId(
            cartId,
            nameof(cartId),
            "Cart ID is required.");

        ValidateId(
            cartItemId,
            nameof(cartItemId),
            "Cart item ID is required.");

        return await dbContext.Set<CartItem>()
            .SingleOrDefaultAsync(
                item =>
                    item.CartId == cartId &&
                    item.Id == cartItemId,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CartVariantDetails?> GetActiveVariantDetailsAsync(
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
            select new CartVariantDetails(
                variant.Id,
                product.Id,
                product.Name,
                variant.Name,
                variant.Quantity.Value,
                variant.Quantity.Unit,
                variant.Price.Amount,
                variant.Price.Currency))
            .SingleOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Add(Cart cart)
    {
        ArgumentNullException.ThrowIfNull(cart);

        dbContext.Set<Cart>().Add(cart);
    }

    /// <inheritdoc />
    public void AddItem(CartItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        dbContext.Set<CartItem>().Add(item);
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