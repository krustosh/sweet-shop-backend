namespace SweetShop.Domain.Enums;

/// <summary>
/// Defines the financial status of an order payment.
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// Payment has not yet been completed.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Payment processing is in progress.
    /// </summary>
    Processing = 2,

    /// <summary>
    /// Payment completed successfully.
    /// </summary>
    Success = 3,

    /// <summary>
    /// Payment failed.
    /// </summary>
    Failed = 4,

    /// <summary>
    /// Payment was cancelled.
    /// </summary>
    Cancelled = 5,

    /// <summary>
    /// Payment was fully refunded.
    /// </summary>
    Refunded = 6,

    /// <summary>
    /// Payment was partially refunded.
    /// </summary>
    PartiallyRefunded = 7
}