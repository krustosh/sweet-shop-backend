namespace SweetShop.Application.Features.Categories.Requests;

/// <summary>
/// Represents a request to update an existing category.
/// </summary>
/// <param name="Name"></param>
/// <param name="Description"></param>
/// <param name="ImageUrl"></param>
/// <param name="DisplayOrder"></param>
public sealed record UpdateCategoryRequest(
    string Name,
    string? Description,
    string? ImageUrl,
    int DisplayOrder);