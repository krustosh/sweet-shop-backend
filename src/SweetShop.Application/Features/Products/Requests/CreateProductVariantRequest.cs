using SweetShop.Domain.Enums;

namespace SweetShop.Application.Features.Products.Requests;

/// <summary>
/// Represents a request to create a product variant.
/// </summary>
public sealed record CreateProductVariantRequest(
    string Name,
    decimal Quantity,
    ProductUnit Unit,
    decimal PriceAmount,
    int DisplayOrder);