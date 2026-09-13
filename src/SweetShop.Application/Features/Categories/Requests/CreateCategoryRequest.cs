namespace SweetShop.Application.Features.Categories.Requests;

/// <summary>
/// Represents a request to create a new category.
/// </summary>
/// <param name="Name">The name of the category.</param>
/// <param name="Description">The description of the category.</param>
/// <param name="ImageUrl">The URL of the category's image.</param>
/// <param name="DisplayOrder">The display order of the category.</param>
public sealed record CreateCategoryRequest(
    string Name,
    string? Description,
    string? ImageUrl,
    int DisplayOrder);