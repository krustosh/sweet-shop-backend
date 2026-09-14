using Microsoft.EntityFrameworkCore;
using SweetShop.Application.Features.ProductImages;
using SweetShop.Domain.Entities;
using SweetShop.Infrastructure.Persistence.Context;

namespace SweetShop.Infrastructure.Persistence.Stores;

/// <summary>
/// Provides persistence operations for product images.
/// </summary>
public sealed class ProductImageStore : IProductImageStore
{
    private readonly SweetShopDbContext dbContext;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbContext"></param>
    public ProductImageStore(SweetShopDbContext dbContext)
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
    public async Task<IReadOnlyCollection<ProductImage>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        ValidateProductId(productId);

        return await dbContext.Set<ProductImage>()
            .Where(image => image.ProductId == productId)
            .OrderBy(image => image.DisplayOrder)
            .ThenBy(image => image.Id)
            .ToArrayAsync(cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="imageId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProductImage?> GetByIdAsync(
        Guid productId,
        Guid imageId,
        CancellationToken cancellationToken)
    {
        ValidateProductId(productId);
        ValidateImageId(imageId);

        return await dbContext.Set<ProductImage>()
            .SingleOrDefaultAsync(
                image =>
                    image.ProductId == productId &&
                    image.Id == imageId,
                cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProductImage?> GetPrimaryByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        ValidateProductId(productId);

        return await dbContext.Set<ProductImage>()
            .SingleOrDefaultAsync(
                image =>
                    image.ProductId == productId &&
                    image.IsPrimary,
                cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="image"></param>
    public void Add(ProductImage image)
    {
        ArgumentNullException.ThrowIfNull(image);

        dbContext.Set<ProductImage>().Add(image);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="image"></param>
    public void Remove(ProductImage image)
    {
        ArgumentNullException.ThrowIfNull(image);

        dbContext.Set<ProductImage>().Remove(image);
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

    private static void ValidateImageId(Guid imageId)
    {
        if (imageId == Guid.Empty)
        {
            throw new ArgumentException(
                "Image ID is required.",
                nameof(imageId));
        }
    }
}