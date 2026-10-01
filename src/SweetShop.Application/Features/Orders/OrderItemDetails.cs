using SweetShop.Domain.Enums;

namespace SweetShop.Application.Features.Orders;

/// <summary>
/// Represents the current product and variant information required when
/// creating an order from a cart item.
/// </summary>
/// <param name="ProductVariantId">The product variant identifier.</param>
/// <param name="ProductId">The product identifier.</param>
/// <param name="ProductName">The current product name.</param>
/// <param name="VariantName">The current variant name.</param>
/// <param name="Unit">The selling unit.</param>
/// <param name="PriceAmount">The current server-authoritative unit price.</param>
/// <param name="Currency">The currency of the price.</param>
public sealed record OrderItemDetails(
    Guid ProductVariantId,
    Guid ProductId,
    string ProductName,
    string VariantName,
    ProductUnit Unit,
    decimal PriceAmount,
    string Currency);