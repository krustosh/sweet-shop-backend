using SweetShop.Domain.Entities;

namespace SweetShop.Application.Features.Products;

/// <summary>
/// Defines persistence operations for product variants.
/// </summary>
public interface IProductVariantStore
{
    /// <summary>
    ///     
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<ProductVariant>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<ProductVariant>> GetActiveByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="variantId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ProductVariant?> GetByIdAsync(
        Guid productId,
        Guid variantId,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ProductVariant?> GetByNameAsync(
        Guid productId,
        string name,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="variant"></param>
    void Add(ProductVariant variant);
}