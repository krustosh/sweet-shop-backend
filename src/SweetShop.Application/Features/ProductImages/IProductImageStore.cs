using SweetShop.Domain.Entities;

namespace SweetShop.Application.Features.ProductImages;

/// <summary>
/// Provides persistence operations for product images.
/// </summary>
public interface IProductImageStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<ProductImage>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="imageId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ProductImage?> GetByIdAsync(
        Guid productId,
        Guid imageId,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ProductImage?> GetPrimaryByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="image"></param>
    void Add(ProductImage image);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="image"></param>
    void Remove(ProductImage image);
}