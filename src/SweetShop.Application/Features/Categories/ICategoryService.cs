using SweetShop.Application.Features.Categories.Requests;
using SweetShop.Application.Features.Categories.Responses;

namespace SweetShop.Application.Features.Categories;

/// <summary>
/// Defines the contract for a service that manages categories in the application.
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Gets a collection of all categories in the application.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<CategoryResponse>> GetCategoriesAsync(
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets a collection of all active categories in the application.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<CategoryResponse>> GetActiveCategoriesAsync(
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets a category by its unique identifier.
    /// </summary>
    /// <param name="categoryId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CategoryResponse?> GetCategoryByIdAsync(
        Guid categoryId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new category in the application.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CategoryResponse> CreateCategoryAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing category in the application.
    /// </summary>
    /// <param name="categoryId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CategoryResponse?> UpdateCategoryAsync(
        Guid categoryId,
        UpdateCategoryRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Deactivates a category in the application.
    /// </summary>
    /// <param name="categoryId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> DeactivateCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken);
}