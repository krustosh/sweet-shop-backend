using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetShop.Api.Common.Models;
using SweetShop.Application.Features.ProductImages;
using SweetShop.Application.Features.ProductImages.Requests;
using SweetShop.Application.Features.ProductImages.Responses;
using SweetShop.Domain.Enums;

namespace SweetShop.Api.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
[Authorize(Roles = nameof(UserRole.Admin))]
[Route("api/v1/admin/products/{productId:guid}/images")]
public sealed class AdminProductImageController : ControllerBase
{
    private readonly IProductImageService productImageService;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productImageService"></param>
    public AdminProductImageController(
        IProductImageService productImageService)
    {
        ArgumentNullException.ThrowIfNull(productImageService);

        this.productImageService = productImageService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(
        typeof(ApiResponse<IReadOnlyCollection<ProductImageResponse>>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetImages(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var response = await productImageService.GetImagesAsync(
            productId,
            activeProductOnly: false,
            cancellationToken);

        return Ok(new ApiResponse<IReadOnlyCollection<ProductImageResponse>>(
            response));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="imageId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{imageId:guid}")]
    [ProducesResponseType(
        typeof(ApiResponse<ProductImageResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetImage(
        Guid productId,
        Guid imageId,
        CancellationToken cancellationToken)
    {
        var response = await productImageService.GetImageByIdAsync(
            productId,
            imageId,
            cancellationToken);

        if (response is null)
        {
            return NotFound(new ApiErrorResponse(
                new ApiError(
                    "PRODUCT_IMAGE_NOT_FOUND",
                    "Product image was not found.",
                    Array.Empty<ApiErrorDetail>())));
        }

        return Ok(new ApiResponse<ProductImageResponse>(response));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(
        typeof(ApiResponse<ProductImageResponse>),
        StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateImage(
        Guid productId,
        CreateProductImageRequest request,
        CancellationToken cancellationToken)
    {
        var response = await productImageService.CreateImageAsync(
            productId,
            request,
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            new ApiResponse<ProductImageResponse>(response));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="imageId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{imageId:guid}")]
    [ProducesResponseType(
        typeof(ApiResponse<ProductImageResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateImage(
        Guid productId,
        Guid imageId,
        UpdateProductImageRequest request,
        CancellationToken cancellationToken)
    {
        var response = await productImageService.UpdateImageAsync(
            productId,
            imageId,
            request,
            cancellationToken);

        if (response is null)
        {
            return NotFound(new ApiErrorResponse(
                new ApiError(
                    "PRODUCT_IMAGE_NOT_FOUND",
                    "Product image was not found.",
                    Array.Empty<ApiErrorDetail>())));
        }

        return Ok(new ApiResponse<ProductImageResponse>(response));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="imageId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{imageId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteImage(
        Guid productId,
        Guid imageId,
        CancellationToken cancellationToken)
    {
        var deleted = await productImageService.DeleteImageAsync(
            productId,
            imageId,
            cancellationToken);

        if (!deleted)
        {
            return NotFound(new ApiErrorResponse(
                new ApiError(
                    "PRODUCT_IMAGE_NOT_FOUND",
                    "Product image was not found.",
                    Array.Empty<ApiErrorDetail>())));
        }

        return NoContent();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="imageId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{imageId:guid}/primary")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetPrimaryImage(
        Guid productId,
        Guid imageId,
        CancellationToken cancellationToken)
    {
        var updated = await productImageService.SetPrimaryImageAsync(
            productId,
            imageId,
            cancellationToken);

        if (!updated)
        {
            return NotFound(new ApiErrorResponse(
                new ApiError(
                    "PRODUCT_IMAGE_NOT_FOUND",
                    "Product image was not found.",
                    Array.Empty<ApiErrorDetail>())));
        }

        return NoContent();
    }
}