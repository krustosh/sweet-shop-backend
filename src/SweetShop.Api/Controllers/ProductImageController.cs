using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetShop.Api.Common.Models;
using SweetShop.Application.Features.ProductImages;
using SweetShop.Application.Features.ProductImages.Responses;

namespace SweetShop.Api.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/products/{productId:guid}/images")]
public sealed class ProductImageController : ControllerBase
{
    private readonly IProductImageService productImageService;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productImageService"></param>
    public ProductImageController(
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
            activeProductOnly: true,
            cancellationToken);

        return Ok(new ApiResponse<IReadOnlyCollection<ProductImageResponse>>(
            response));
    }
}