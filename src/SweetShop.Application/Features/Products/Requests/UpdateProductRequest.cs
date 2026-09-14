namespace SweetShop.Application.Features.Products.Requests;

/// <summary>
/// Represents a request to update a product.
/// </summary>
public sealed record UpdateProductRequest(
    Guid CategoryId,
    string Name,
    string? Description,
    int DisplayOrder);