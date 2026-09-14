using Microsoft.EntityFrameworkCore;
using SweetShop.Application.Features.Products;
using SweetShop.Domain.Entities;
using SweetShop.Domain.Enums;
using SweetShop.Infrastructure.Persistence.Context;

namespace SweetShop.Infrastructure.Persistence.Stores;

/// <summary>
/// Provides persistence operations for product variants.
/// </summary>
public sealed class ProductVariantStore : IProductVariantStore
{
    private readonly SweetShopDbContext dbContext;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbContext"></param>
    public ProductVariantStore(SweetShopDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        this.dbContext = dbContext;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IReadOnlyCollection<ProductVariant>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        ValidateProductId(productId);

        return await dbContext.Set<ProductVariant>()
            .Where(variant => variant.ProductId == productId)
            .OrderBy(variant => variant.DisplayOrder)
            .ThenBy(variant => variant.Name)
            .ToArrayAsync(cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IReadOnlyCollection<ProductVariant>> GetActiveByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        ValidateProductId(productId);

        return await dbContext.Set<ProductVariant>()
            .Where(variant =>
                variant.ProductId == productId &&
                variant.Status == ProductVariantStatus.Active)
            .OrderBy(variant => variant.DisplayOrder)
            .ThenBy(variant => variant.Name)
            .ToArrayAsync(cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="variantId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<ProductVariant?> GetByIdAsync(
        Guid productId,
        Guid variantId,
        CancellationToken cancellationToken)
    {
        ValidateProductId(productId);

        if (variantId == Guid.Empty)
        {
            throw new ArgumentException(
                "Variant ID is required.",
                nameof(variantId));
        }

        return await dbContext.Set<ProductVariant>()
            .SingleOrDefaultAsync(
                variant =>
                    variant.ProductId == productId &&
                    variant.Id == variantId,
                cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<ProductVariant?> GetByNameAsync(
        Guid productId,
        string name,
        CancellationToken cancellationToken)
    {
        ValidateProductId(productId);

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Variant name is required.",
                nameof(name));
        }

        var normalizedName = name.Trim();

        return await dbContext.Set<ProductVariant>()
            .SingleOrDefaultAsync(
                variant =>
                    variant.ProductId == productId &&
                    variant.Name == normalizedName,
                cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="variant"></param>
    public void Add(ProductVariant variant)
    {
        ArgumentNullException.ThrowIfNull(variant);

        dbContext.Set<ProductVariant>().Add(variant);
    }

    private static void ValidateProductId(Guid productId)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException(
                "Product ID is required.",
                nameof(productId));
        }
    }
}