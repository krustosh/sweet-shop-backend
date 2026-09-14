using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetShop.Api.Common.Models;
using SweetShop.Application.Features.Products;
using SweetShop.Application.Features.Products.Requests;
using SweetShop.Application.Features.Products.Responses;
using SweetShop.Domain.Enums;

namespace SweetShop.Api.Controllers;

/// <summary>
/// Provides administrative APIs for managing products and product variants.
/// </summary>
[ApiController]
[Authorize(Roles = nameof(UserRole.Admin))]
[Route("api/v1/admin/products")]
public sealed class AdminProductController : ControllerBase
{
    private readonly IProductService productService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminProductController"/> class.
    /// </summary>
    /// <param name="productService">The product application service.</param>
    public AdminProductController(IProductService productService)
    {
        ArgumentNullException.ThrowIfNull(productService);

        this.productService = productService;
    }

    /// <summary>
    /// Gets all products for the current shop.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(
        typeof(ApiResponse<IReadOnlyCollection<ProductResponse>>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
        CancellationToken cancellationToken)
    {
        var response = await productService.GetProductsAsync(
            cancellationToken);

        return Ok(new ApiResponse<IReadOnlyCollection<ProductResponse>>(
            response));
    }

    /// <summary>
    /// Gets a product by identifier for the current shop.
    /// </summary>
    [HttpGet("{productId:guid}")]
    [ProducesResponseType(
        typeof(ApiResponse<ProductResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductById(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var response = await productService.GetProductByIdAsync(
            productId,
            cancellationToken);

        if (response is null)
        {
            return NotFound(new ApiErrorResponse(
                new ApiError(
                    "PRODUCT_NOT_FOUND",
                    "Product was not found.",
                    Array.Empty<ApiErrorDetail>())));
        }

        return Ok(new ApiResponse<ProductResponse>(response));
    }

    /// <summary>
    /// Creates a product for the current shop.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(
        typeof(ApiResponse<ProductResponse>),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var response = await productService.CreateProductAsync(
            request,
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            new ApiResponse<ProductResponse>(response));
    }

    /// <summary>
    /// Updates a product for the current shop.
    /// </summary>
    [HttpPut("{productId:guid}")]
    [ProducesResponseType(
        typeof(ApiResponse<ProductResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(
        Guid productId,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var response = await productService.UpdateProductAsync(
            productId,
            request,
            cancellationToken);

        if (response is null)
        {
            return NotFound(new ApiErrorResponse(
                new ApiError(
                    "PRODUCT_NOT_FOUND",
                    "Product was not found.",
                    Array.Empty<ApiErrorDetail>())));
        }

        return Ok(new ApiResponse<ProductResponse>(response));
    }

    /// <summary>
    /// Deactivates a product for the current shop.
    /// </summary>
    [HttpDelete("{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateProduct(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var deactivated = await productService.DeactivateProductAsync(
            productId,
            cancellationToken);

        if (!deactivated)
        {
            return NotFound(new ApiErrorResponse(
                new ApiError(
                    "PRODUCT_NOT_FOUND",
                    "Product was not found.",
                    Array.Empty<ApiErrorDetail>())));
        }

        return NoContent();
    }

    /// <summary>
    /// Gets all variants for a product.
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
            activeOnly: false,
            cancellationToken);

        return Ok(
            new ApiResponse<IReadOnlyCollection<ProductVariantResponse>>(
                response));
    }

    /// <summary>
    /// Gets a variant by identifier.
    /// </summary>
    [HttpGet("{productId:guid}/variants/{variantId:guid}")]
    [ProducesResponseType(
        typeof(ApiResponse<ProductVariantResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVariantById(
        Guid productId,
        Guid variantId,
        CancellationToken cancellationToken)
    {
        var response = await productService.GetVariantByIdAsync(
            productId,
            variantId,
            cancellationToken);

        if (response is null)
        {
            return NotFound(new ApiErrorResponse(
                new ApiError(
                    "PRODUCT_VARIANT_NOT_FOUND",
                    "Product variant was not found.",
                    Array.Empty<ApiErrorDetail>())));
        }

        return Ok(new ApiResponse<ProductVariantResponse>(response));
    }

    /// <summary>
    /// Creates a variant for a product.
    /// </summary>
    [HttpPost("{productId:guid}/variants")]
    [ProducesResponseType(
        typeof(ApiResponse<ProductVariantResponse>),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateVariant(
        Guid productId,
        [FromBody] CreateProductVariantRequest request,
        CancellationToken cancellationToken)
    {
        var response = await productService.CreateVariantAsync(
            productId,
            request,
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            new ApiResponse<ProductVariantResponse>(response));
    }

    /// <summary>
    /// Updates a variant for a product.
    /// </summary>
    [HttpPut("{productId:guid}/variants/{variantId:guid}")]
    [ProducesResponseType(
        typeof(ApiResponse<ProductVariantResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVariant(
        Guid productId,
        Guid variantId,
        [FromBody] UpdateProductVariantRequest request,
        CancellationToken cancellationToken)
    {
        var response = await productService.UpdateVariantAsync(
            productId,
            variantId,
            request,
            cancellationToken);

        if (response is null)
        {
            return NotFound(new ApiErrorResponse(
                new ApiError(
                    "PRODUCT_VARIANT_NOT_FOUND",
                    "Product variant was not found.",
                    Array.Empty<ApiErrorDetail>())));
        }

        return Ok(new ApiResponse<ProductVariantResponse>(response));
    }

    /// <summary>
    /// Deactivates a variant for a product.
    /// </summary>
    [HttpDelete("{productId:guid}/variants/{variantId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateVariant(
        Guid productId,
        Guid variantId,
        CancellationToken cancellationToken)
    {
        var deactivated = await productService.DeactivateVariantAsync(
            productId,
            variantId,
            cancellationToken);

        if (!deactivated)
        {
            return NotFound(new ApiErrorResponse(
                new ApiError(
                    "PRODUCT_VARIANT_NOT_FOUND",
                    "Product variant was not found.",
                    Array.Empty<ApiErrorDetail>())));
        }

        return NoContent();
    }

/// <summary>
/// 
/// </summary>
/// <param name="productId"></param>
/// <param name="cancellationToken"></param>
/// <returns></returns>
[HttpPut("{productId:guid}/activate")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
public async Task<IActionResult> ActivateProduct(
    Guid productId,
    CancellationToken cancellationToken)
{
    var activated = await productService.ActivateProductAsync(
        productId,
        cancellationToken);

    if (!activated)
    {
        return NotFound(new ApiErrorResponse(
            new ApiError(
                "PRODUCT_NOT_FOUND",
                "Product was not found.",
                Array.Empty<ApiErrorDetail>())));
    }

    return NoContent();
}

/// <summary>
/// 
/// </summary>
/// <param name="productId"></param>
/// <param name="variantId"></param>
/// <param name="cancellationToken"></param>
/// <returns></returns>
[HttpPut("{productId:guid}/variants/{variantId:guid}/activate")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
public async Task<IActionResult> ActivateVariant(
    Guid productId,
    Guid variantId,
    CancellationToken cancellationToken)
{
    var activated = await productService.ActivateVariantAsync(
        productId,
        variantId,
        cancellationToken);

    if (!activated)
    {
        return NotFound(new ApiErrorResponse(
            new ApiError(
                "PRODUCT_VARIANT_NOT_FOUND",
                "Product variant was not found.",
                Array.Empty<ApiErrorDetail>())));
    }

    return NoContent();
}
}