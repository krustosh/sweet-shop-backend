using SweetShop.Domain.Enums;

namespace SweetShop.Application.Features.Products.Responses;

/// <summary>
/// Represents a product response.
/// </summary>
public sealed record ProductResponse(
    Guid Id,
    Guid ShopId,
    Guid CategoryId,
    string Name,
    string? Description,
    int DisplayOrder,
    ProductStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt);