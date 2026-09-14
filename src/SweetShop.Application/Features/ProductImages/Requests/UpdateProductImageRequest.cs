namespace SweetShop.Application.Features.ProductImages.Requests;

/// <summary>
/// Represents a request to update a product image.
/// </summary>
public sealed record UpdateProductImageRequest(
string Url,
string? AltText,
int DisplayOrder);
