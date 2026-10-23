namespace SweetShop.Application.Features.Payments.Responses;

/// <summary>
/// Represents payment information returned to an API client.
/// </summary>
public sealed record PaymentResponse
{
    /// <summary>
    /// Gets the payment identifier.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Gets the associated order identifier.
    /// </summary>
    public required Guid OrderId { get; init; }

    /// <summary>
    /// Gets the payment reference number.
    /// </summary>
    public required string PaymentNumber { get; init; }

    /// <summary>
    /// Gets the payment method.
    /// </summary>
    public required string Method { get; init; }

    /// <summary>
    /// Gets the current payment status.
    /// </summary>
    public required string Status { get; init; }

    /// <summary>
    /// Gets the payment amount calculated by the server.
    /// </summary>
    public required decimal Amount { get; init; }

    /// <summary>
    /// Gets the ISO currency code.
    /// </summary>
    public required string Currency { get; init; }

    /// <summary>
    /// Gets the payment provider, when applicable.
    /// </summary>
    public string? Provider { get; init; }

    /// <summary>
    /// Gets the provider transaction identifier, when available.
    /// </summary>
    public string? ProviderTransactionId { get; init; }

    /// <summary>
    /// Gets the time at which payment succeeded, in UTC.
    /// </summary>
    public DateTime? PaidAt { get; init; }

    /// <summary>
    /// Gets the UTC time at which the payment record was created.
    /// </summary>
    public required DateTime CreatedAt { get; init; }
}