namespace SweetShop.Application.Features.Payments.Requests;

/// <summary>
/// Represents a request to initiate payment for an existing order.
/// The payment amount is calculated from the saved order, not supplied by the customer.
/// </summary>
public sealed record InitiatePaymentRequest
{
    /// <summary>
    /// Gets the requested payment method.
    /// </summary>
    public required string Method { get; init; }
}