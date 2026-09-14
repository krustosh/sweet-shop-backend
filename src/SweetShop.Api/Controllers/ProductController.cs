using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetShop.Api.Common.Models;
using SweetShop.Application.Features.Products;
using SweetShop.Application.Features.Products.Responses;

namespace SweetShop.Api.Controllers;

/// <summary>
/// Provides customer-facing APIs for products.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/products")]
public sealed class ProductController : ControllerBase
{
    private readonly IProductService productService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductController"/> class.
    /// </summary>
    /// <param name="productService">The product application service.</param>
    public ProductController(IProductService productService)
    {
        ArgumentNullException.ThrowIfNull(productService);

        this.productService = productService;
    }

    /// <summary>
    /// Gets all active products for the current shop.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(
        typeof(ApiResponse<IReadOnlyCollection<ProductResponse>>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
        CancellationToken cancellationToken)
    {
        var response = await productService.GetActiveProductsAsync(
            cancellationToken);

        return Ok(new ApiResponse<IReadOnlyCollection<ProductResponse>>(
            response));
    }

    /// <summary>
    /// Gets all active variants for a product.
    /// </summary>
    [HttpGet("{productId:guid}/variants")]
    [ProducesResponseType(
        typeof(ApiResponse<IReadOnlyCollection<ProductVariantResponse>>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVariants(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var response = await productService.GetVariantsAsync(
            productId,
            activeOnly: true,
            cancellationToken);

        return Ok(
            new ApiResponse<IReadOnlyCollection<ProductVariantResponse>>(
                response));
    }
}