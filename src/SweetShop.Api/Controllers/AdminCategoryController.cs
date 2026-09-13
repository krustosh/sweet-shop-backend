using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetShop.Api.Common.Models;
using SweetShop.Application.Features.Categories;
using SweetShop.Application.Features.Categories.Requests;
using SweetShop.Application.Features.Categories.Responses;
using SweetShop.Domain.Enums;

namespace SweetShop.Api.Controllers;

/// <summary>
/// Provides administrative APIs for managing product categories.
/// </summary>
[ApiController]
[Authorize(Roles = nameof(UserRole.Admin))]
[Route("api/v1/admin/categories")]
public sealed class AdminCategoryController : ControllerBase
{
    private readonly ICategoryService categoryService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminCategoryController"/> class.
    /// </summary>
    /// <param name="categoryService">The category application service.</param>
    public AdminCategoryController(ICategoryService categoryService)
    {
        ArgumentNullException.ThrowIfNull(categoryService);

        this.categoryService = categoryService;
    }

    /// <summary>
    /// Gets all categories for the current shop.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(
        typeof(ApiResponse<IReadOnlyCollection<CategoryResponse>>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories(
        CancellationToken cancellationToken)
    {
        var response = await categoryService.GetCategoriesAsync(
            cancellationToken);

        return Ok(new ApiResponse<IReadOnlyCollection<CategoryResponse>>(
            response));
    }

    /// <summary>
    /// Gets a category by identifier for the current shop.
    /// </summary>
    [HttpGet("{categoryId:guid}")]
    [ProducesResponseType(
        typeof(ApiResponse<CategoryResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategoryById(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var response = await categoryService.GetCategoryByIdAsync(
            categoryId,
            cancellationToken);

        if (response is null)
        {
            return NotFound(new ApiErrorResponse(
                new ApiError(
                    "CATEGORY_NOT_FOUND",
                    "Category was not found.",
                    Array.Empty<ApiErrorDetail>())));
        }

        return Ok(new ApiResponse<CategoryResponse>(response));
    }

    /// <summary>
    /// Creates a category for the current shop.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(
        typeof(ApiResponse<CategoryResponse>),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var response = await categoryService.CreateCategoryAsync(
            request,
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            new ApiResponse<CategoryResponse>(response));
    }

    /// <summary>
    /// Updates a category for the current shop.
    /// </summary>
    [HttpPut("{categoryId:guid}")]
    [ProducesResponseType(
        typeof(ApiResponse<CategoryResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCategory(
        Guid categoryId,
        [FromBody] UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var response = await categoryService.UpdateCategoryAsync(
            categoryId,
            request,
            cancellationToken);

        if (response is null)
        {
            return NotFound(new ApiErrorResponse(
                new ApiError(
                    "CATEGORY_NOT_FOUND",
                    "Category was not found.",
                    Array.Empty<ApiErrorDetail>())));
        }

        return Ok(new ApiResponse<CategoryResponse>(response));
    }

    /// <summary>
    /// Deactivates a category for the current shop.
    /// </summary>
    [HttpDelete("{categoryId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateCategory(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var deactivated = await categoryService.DeactivateCategoryAsync(
            categoryId,
            cancellationToken);

        if (!deactivated)
        {
            return NotFound(new ApiErrorResponse(
                new ApiError(
                    "CATEGORY_NOT_FOUND",
                    "Category was not found.",
                    Array.Empty<ApiErrorDetail>())));
        }

        return NoContent();
    }
}