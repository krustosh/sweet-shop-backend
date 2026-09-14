namespace SweetShop.Application.Features.Products.Requests;

/// <summary>
/// Represents a request to update mutable product variant information.
/// </summary>
public sealed record UpdateProductVariantRequest(
    string Name,
    decimal PriceAmount,
    int DisplayOrder);