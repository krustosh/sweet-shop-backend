using SweetShop.Domain.Enums;

namespace SweetShop.Application.Features.Categories.Responses;

/// <summary>
/// Represents a response containing information about a category.
/// </summary>
/// <param name="Id">The unique identifier of the category.</param>
/// <param name="ShopId">The unique identifier of the shop to which the category belongs.</param>
/// <param name="Name">The name of the category.</param>
/// <param name="Description">The description of the category.</param>
/// <param name="ImageUrl">The URL of the category's image.</param>
/// <param name="DisplayOrder">The display order of the category.</param>
/// <param name="Status">The status of the category.</param>
/// <param name="CreatedAt">The date and time when the category was created.</param>
/// <param name="UpdatedAt">The date and time when the category was last updated.</param>
public sealed record CategoryResponse(
    Guid Id,
    Guid ShopId,
    string Name,
    string? Description,
    string? ImageUrl,
    int DisplayOrder,
    CategoryStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt);