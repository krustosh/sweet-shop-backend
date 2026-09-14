using SweetShop.Application.Features.Products.Requests;
using SweetShop.Application.Features.Products.Responses;

namespace SweetShop.Application.Features.Products;

/// <summary>
/// 
/// </summary>
public interface IProductService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<ProductResponse>> GetProductsAsync(
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<ProductResponse>> GetActiveProductsAsync(
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ProductResponse?> GetProductByIdAsync(
        Guid productId,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ProductResponse> CreateProductAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ProductResponse?> UpdateProductAsync(
        Guid productId,
        UpdateProductRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> DeactivateProductAsync(
        Guid productId,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> ActivateProductAsync(
        Guid productId,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="activeOnly"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<ProductVariantResponse>> GetVariantsAsync(
        Guid productId,
        bool activeOnly,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="variantId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ProductVariantResponse?> GetVariantByIdAsync(
        Guid productId,
        Guid variantId,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ProductVariantResponse> CreateVariantAsync(
        Guid productId,
        CreateProductVariantRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="variantId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ProductVariantResponse?> UpdateVariantAsync(
        Guid productId,
        Guid variantId,
        UpdateProductVariantRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="variantId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> DeactivateVariantAsync(
        Guid productId,
        Guid variantId,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="variantId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> ActivateVariantAsync(
        Guid productId,
        Guid variantId,
        CancellationToken cancellationToken);
}
