using SweetShop.Domain.Entities;

namespace SweetShop.Application.Features.Products;

/// <summary>
/// Defines persistence operations for products.
/// </summary>
public interface IProductStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="shopId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<Product>> GetByShopIdAsync(
        Guid shopId,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="shopId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<Product>> GetActiveByShopIdAsync(
        Guid shopId,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="shopId"></param>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Product?> GetByIdAsync(
        Guid shopId,
        Guid productId,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="shopId"></param>
    /// <param name="categoryId"></param>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Product?> GetByNameAsync(
        Guid shopId,
        Guid categoryId,
        string name,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="product"></param>
    void Add(Product product);
}