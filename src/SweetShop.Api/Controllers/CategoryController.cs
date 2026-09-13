using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetShop.Api.Common.Models;
using SweetShop.Application.Features.Categories;
using SweetShop.Application.Features.Categories.Responses;

namespace SweetShop.Api.Controllers;

/// <summary>
/// Provides customer-facing APIs for product categories.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/categories")]
public sealed class CategoryController : ControllerBase
{
    private readonly ICategoryService categoryService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoryController"/> class.
    /// </summary>
    /// <param name="categoryService">The category application service.</param>
    public CategoryController(ICategoryService categoryService)
    {
        ArgumentNullException.ThrowIfNull(categoryService);

        this.categoryService = categoryService;
    }

    /// <summary>
    /// Gets all active categories for the current shop.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(
        typeof(ApiResponse<IReadOnlyCollection<CategoryResponse>>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories(
        CancellationToken cancellationToken)
    {
        var response = await categoryService.GetActiveCategoriesAsync(
            cancellationToken);

        return Ok(new ApiResponse<IReadOnlyCollection<CategoryResponse>>(
            response));
    }
}