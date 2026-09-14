namespace SweetShop.Application.Features.Products.Requests;

/// <summary>
/// Represents a request to create a product.
/// </summary>
public sealed record CreateProductRequest(
    Guid CategoryId,
    string Name,
    string? Description,
    int DisplayOrder);