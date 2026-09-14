namespace SweetShop.Application.Features.ProductImages.Responses;

/// <summary>
/// Represents a product image response.
/// </summary>
public sealed record ProductImageResponse(
    Guid Id,
    Guid ProductId,
    string Url,
    string? AltText,
    int DisplayOrder,
    bool IsPrimary,
    DateTime CreatedAt,
    DateTime UpdatedAt);