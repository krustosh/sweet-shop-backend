using SweetShop.Application.Features.ProductImages.Requests;
using SweetShop.Application.Features.ProductImages.Responses;

namespace SweetShop.Application.Features.ProductImages;

/// <summary>
/// Provides product image management operations.
/// </summary>
public interface IProductImageService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="activeProductOnly"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<ProductImageResponse>> GetImagesAsync(
        Guid productId,
        bool activeProductOnly,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="imageId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ProductImageResponse?> GetImageByIdAsync(
        Guid productId,
        Guid imageId,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ProductImageResponse> CreateImageAsync(
        Guid productId,
        CreateProductImageRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="imageId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ProductImageResponse?> UpdateImageAsync(
        Guid productId,
        Guid imageId,
        UpdateProductImageRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="imageId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> DeleteImageAsync(
        Guid productId,
        Guid imageId,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="imageId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetPrimaryImageAsync(
        Guid productId,
        Guid imageId,
        CancellationToken cancellationToken);
}