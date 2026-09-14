namespace SweetShop.Application.Features.ProductImages.Requests;

/// <summary>
/// Represents a request to create a product image.
/// </summary>
public sealed record CreateProductImageRequest(
    string Url,
    string? AltText,
    int DisplayOrder,
    bool IsPrimary);