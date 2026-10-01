using SweetShop.Domain.Enums;

namespace SweetShop.Application.Features.Cart.Responses;

/// <summary>
/// Represents a cart item returned to a customer.
/// </summary>
public sealed record CartItemResponse(
    Guid Id,
    Guid ProductVariantId,
    Guid ProductId,
    string ProductName,
    string VariantName,
    decimal VariantQuantity,
    ProductUnit Unit,
    decimal Quantity,
    decimal UnitPrice,
    decimal LineTotal,
    string Currency,
    DateTime CreatedAt,
    DateTime UpdatedAt);
