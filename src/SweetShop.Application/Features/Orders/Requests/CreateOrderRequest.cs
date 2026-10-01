using SweetShop.Domain.Enums;

namespace SweetShop.Application.Features.Orders.Requests;

/// <summary>
/// Represents a request to create an order from the authenticated customer's active cart.
/// </summary>
/// <param name="FulfillmentType">The requested fulfillment method.</param>
/// <param name="DeliveryAddressId">
/// The saved delivery address identifier. Required for delivery orders and must be omitted
/// for pickup orders.
/// </param>
/// <param name="CustomerNote">The optional note supplied by the customer.</param>
public sealed record CreateOrderRequest(
    FulfillmentType FulfillmentType,
    Guid? DeliveryAddressId,
    string? CustomerNote);