using SweetShop.Domain.Enums;

namespace SweetShop.Application.Features.Cart;

/// <summary>
/// Represents product-variant information required to build a cart response.
/// </summary>
public sealed record CartVariantDetails(
    Guid ProductVariantId,
    Guid ProductId,
    string ProductName,
    string VariantName,
    decimal VariantQuantity,
    ProductUnit Unit,
    decimal PriceAmount,
    string Currency);
