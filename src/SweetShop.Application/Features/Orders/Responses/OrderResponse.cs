using SweetShop.Domain.Enums;

namespace SweetShop.Application.Features.Orders.Responses;

/// <summary>
/// Represents an order returned to the customer.
/// </summary>
/// <param name="Id">The order identifier.</param>
/// <param name="OrderNumber">The human-readable order number.</param>
/// <param name="CustomerId">The customer identifier.</param>
/// <param name="FulfillmentType">The fulfillment method.</param>
/// <param name="Status">The current order status.</param>
/// <param name="PaymentStatus">The current payment status.</param>
/// <param name="Subtotal">The order subtotal.</param>
/// <param name="DeliveryFee">The delivery fee.</param>
/// <param name="DiscountAmount">The discount amount.</param>
/// <param name="TotalAmount">The final order total.</param>
/// <param name="Currency">The monetary currency.</param>
/// <param name="CustomerNote">The customer note.</param>
/// <param name="DeliveryAddress">The immutable delivery address snapshot.</param>
/// <param name="PickupDetails">The pickup details.</param>
/// <param name="Items">The immutable order items.</param>
/// <param name="PlacedAt">The order placement timestamp.</param>
/// <param name="AcceptedAt">The acceptance timestamp.</param>
/// <param name="PreparingAt">The preparation timestamp.</param>
/// <param name="ReadyAt">The ready timestamp.</param>
/// <param name="CompletedAt">The completion timestamp.</param>
/// <param name="CancelledAt">The cancellation timestamp.</param>
/// <param name="CancellationReason">The cancellation reason.</param>
/// <param name="CreatedAt">The entity creation timestamp.</param>
/// <param name="UpdatedAt">The entity update timestamp.</param>
public sealed record OrderResponse(
    Guid Id,
    string OrderNumber,
    Guid CustomerId,
    FulfillmentType FulfillmentType,
    OrderStatus Status,
    PaymentStatus PaymentStatus,
    decimal Subtotal,
    decimal DeliveryFee,
    decimal DiscountAmount,
    decimal TotalAmount,
    string Currency,
    string? CustomerNote,
    OrderAddressResponse? DeliveryAddress,
    string? PickupDetails,
    IReadOnlyCollection<OrderItemResponse> Items,
    DateTime? PlacedAt,
    DateTime? AcceptedAt,
    DateTime? PreparingAt,
    DateTime? ReadyAt,
    DateTime? CompletedAt,
    DateTime? CancelledAt,
    string? CancellationReason,
    DateTime CreatedAt,
    DateTime UpdatedAt);

/// <summary>
/// Represents the immutable delivery address captured with an order.
/// </summary>
/// <param name="RecipientName">The recipient's name.</param>
/// <param name="MobileNumber">The recipient's mobile number.</param>
/// <param name="AddressLine1">The first address line.</param>
/// <param name="AddressLine2">The second address line.</param>
/// <param name="Landmark">The landmark.</param>
/// <param name="Area">The area or locality.</param>
/// <param name="City">The city.</param>
/// <param name="State">The state.</param>
/// <param name="PostalCode">The postal code.</param>
/// <param name="Latitude">The latitude coordinate.</param>
/// <param name="Longitude">The longitude coordinate.</param>
public sealed record OrderAddressResponse(
    string RecipientName,
    string MobileNumber,
    string AddressLine1,
    string? AddressLine2,
    string? Landmark,
    string Area,
    string City,
    string State,
    string PostalCode,
    decimal? Latitude,
    decimal? Longitude);