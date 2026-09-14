using SweetShop.Domain.Enums;

namespace SweetShop.Application.Features.Products.Responses;

/// <summary>
/// Represents a product variant response.
/// </summary>
public sealed record ProductVariantResponse(
    Guid Id,
    Guid ProductId,
    string Name,
    decimal Quantity,
    ProductUnit Unit,
    decimal PriceAmount,
    string Currency,
    int DisplayOrder,
    ProductVariantStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt);