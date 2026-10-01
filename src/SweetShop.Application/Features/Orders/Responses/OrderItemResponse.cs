using SweetShop.Domain.Enums;

namespace SweetShop.Application.Features.Orders.Responses;

/// <summary>
/// Represents an immutable order item returned to the customer.
/// </summary>
/// <param name="Id">The order item identifier.</param>
/// <param name="ProductId">The original product identifier.</param>
/// <param name="ProductVariantId">The original product variant identifier.</param>
/// <param name="ProductName">The product name captured at order time.</param>
/// <param name="VariantName">The variant name captured at order time.</param>
/// <param name="Quantity">The ordered quantity.</param>
/// <param name="Unit">The selling unit.</param>
/// <param name="UnitPrice">The unit price captured at order time.</param>
/// <param name="LineTotal">The calculated line total.</param>
/// <param name="Currency">The monetary currency.</param>
/// <param name="CreatedAt">The item creation timestamp.</param>
public sealed record OrderItemResponse(
    Guid Id,
    Guid ProductId,
    Guid ProductVariantId,
    string ProductName,
    string VariantName,
    decimal Quantity,
    ProductUnit Unit,
    decimal UnitPrice,
    decimal LineTotal,
    string Currency,
    DateTime CreatedAt);