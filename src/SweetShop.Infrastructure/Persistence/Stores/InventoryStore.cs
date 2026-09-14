using Microsoft.EntityFrameworkCore;
using SweetShop.Application.Features.Inventory;
using SweetShop.Domain.Entities;
using SweetShop.Infrastructure.Persistence.Context;

using InventoryEntity = SweetShop.Domain.Entities.Inventory;

namespace SweetShop.Infrastructure.Persistence.Stores;

/// <summary>
/// Provides persistence operations for inventory.
/// </summary>
public sealed class InventoryStore : IInventoryStore
{
    private readonly SweetShopDbContext dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryStore"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public InventoryStore(SweetShopDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        this.dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<InventoryEntity>> GetByShopIdAsync(
        Guid shopId,
        CancellationToken cancellationToken)
    {
        ValidateShopId(shopId);

        return await (
            from inventory in dbContext.Set<InventoryEntity>()
            join variant in dbContext.Set<ProductVariant>()
                on inventory.ProductVariantId equals variant.Id
            join product in dbContext.Set<Product>()
                on variant.ProductId equals product.Id
            where product.ShopId == shopId
            select inventory)
            .OrderBy(inventory => inventory.ProductVariantId)
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<InventoryEntity?> GetByVariantIdAsync(
        Guid shopId,
        Guid productVariantId,
        CancellationToken cancellationToken)
    {
        ValidateShopId(shopId);
        ValidateProductVariantId(productVariantId);

        return await (
            from inventory in dbContext.Set<InventoryEntity>()
            join variant in dbContext.Set<ProductVariant>()
                on inventory.ProductVariantId equals variant.Id
            join product in dbContext.Set<Product>()
                on variant.ProductId equals product.Id
            where product.ShopId == shopId &&
                  variant.Id == productVariantId
            select inventory)
            .SingleOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> VariantExistsForShopAsync(
        Guid shopId,
        Guid productVariantId,
        CancellationToken cancellationToken)
    {
        ValidateShopId(shopId);
        ValidateProductVariantId(productVariantId);

        return await (
            from variant in dbContext.Set<ProductVariant>()
            join product in dbContext.Set<Product>()
                on variant.ProductId equals product.Id
            where product.ShopId == shopId &&
                  variant.Id == productVariantId
            select variant.Id)
            .AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Add(InventoryEntity inventory)
    {
        ArgumentNullException.ThrowIfNull(inventory);

        dbContext.Set<InventoryEntity>().Add(inventory);
    }

    private static void ValidateShopId(Guid shopId)
    {
        if (shopId == Guid.Empty)
        {
            throw new ArgumentException(
                "Shop ID is required.",
                nameof(shopId));
        }
    }

    private static void ValidateProductVariantId(
        Guid productVariantId)
    {
        if (productVariantId == Guid.Empty)
        {
            throw new ArgumentException(
                "Product variant ID is required.",
                nameof(productVariantId));
        }
    }
}