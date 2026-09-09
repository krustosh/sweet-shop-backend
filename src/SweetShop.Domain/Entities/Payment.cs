using SweetShop.Domain.Common;
using SweetShop.Domain.Enums;
using SweetShop.Domain.ValueObjects;

namespace SweetShop.Domain.Entities;

/// <summary>
/// Represents a payment transaction or payment attempt for an order.
/// </summary>
public sealed class Payment : AuditableEntity
{
    /// <summary>
    /// Gets the identifier of the associated order.
    /// </summary>
    public Guid OrderId { get; private set; }

    /// <summary>
    /// Gets the human-readable payment number.
    /// </summary>
    public string PaymentNumber { get; private set; }

    /// <summary>
    /// Gets the payment method.
    /// </summary>
    public PaymentMethod Method { get; private set; }

    /// <summary>
    /// Gets the payment status.
    /// </summary>
    public PaymentStatus Status { get; private set; }

    /// <summary>
    /// Gets the payment amount.
    /// </summary>
    public Money Amount { get; private set; }

    /// <summary>
    /// Gets the payment provider.
    /// </summary>
    public string? Provider { get; private set; }

    /// <summary>
    /// Gets the provider transaction identifier.
    /// </summary>
    public string? ProviderTransactionId { get; private set; }

    /// <summary>
    /// Gets the provider failure code.
    /// </summary>
    public string? FailureCode { get; private set; }

    /// <summary>
    /// Gets the provider failure message.
    /// </summary>
    public string? FailureMessage { get; private set; }

    /// <summary>
    /// Gets the date and time when the payment was successfully completed.
    /// </summary>
    public DateTime? PaidAt { get; private set; }

/// <summary>
/// Initializes a new instance of the <see cref="Payment"/> class. 
/// </summary>
    private Payment()
    {
        PaymentNumber = string.Empty;
        Amount = new Money(0, "INR");
    }

    /// <summary>
    /// Initializes a new payment.
    /// </summary>
    /// <param name="orderId">The identifier of the associated order.</param>
    /// <param name="paymentNumber">The human-readable payment number.</param>
    /// <param name="method">The payment method.</param>
    /// <param name="amount">The payment amount.</param>
    public Payment(
        Guid orderId,
        string paymentNumber,
        PaymentMethod method,
        Money amount)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException(
                "Order ID cannot be empty.",
                nameof(orderId));
        }

        ArgumentNullException.ThrowIfNull(amount);

        OrderId = orderId;
        PaymentNumber = RequireValue(
            paymentNumber,
            nameof(paymentNumber));
        Method = method;
        Amount = amount;
        Status = PaymentStatus.Pending;
    }

    /// <summary>
    /// Marks the payment as processing.
    /// </summary>
    public void MarkProcessing()
    {
        EnsureNotFinalized();

        Status = PaymentStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the payment as successful.
    /// </summary>
    /// <param name="provider">The optional payment provider.</param>
    /// <param name="providerTransactionId">
    /// The optional provider transaction identifier.
    /// </param>
    public void MarkSuccessful(
        string? provider,
        string? providerTransactionId)
    {
        EnsureNotFinalized();

        Status = PaymentStatus.Success;
        Provider = NormalizeOptional(provider);
        ProviderTransactionId = NormalizeOptional(providerTransactionId);
        PaidAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the payment as failed.
    /// </summary>
    /// <param name="failureCode">The optional failure code.</param>
    /// <param name="failureMessage">The optional failure message.</param>
    public void MarkFailed(
        string? failureCode,
        string? failureMessage)
    {
        EnsureNotFinalized();

        Status = PaymentStatus.Failed;
        FailureCode = NormalizeOptional(failureCode);
        FailureMessage = NormalizeOptional(failureMessage);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the payment as cancelled.
    /// </summary>
    public void Cancel()
    {
        EnsureNotFinalized();

        Status = PaymentStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the payment as fully refunded.
    /// </summary>
    public void Refund()
    {
        if (Status != PaymentStatus.Success)
        {
            throw new InvalidOperationException(
                "Only successful payments can be refunded.");
        }

        Status = PaymentStatus.Refunded;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the payment as partially refunded.
    /// </summary>
    public void PartiallyRefund()
    {
        if (Status != PaymentStatus.Success)
        {
            throw new InvalidOperationException(
                "Only successful payments can be partially refunded.");
        }

        Status = PaymentStatus.PartiallyRefunded;
        UpdatedAt = DateTime.UtcNow;
    }

    private void EnsureNotFinalized()
    {
        if (Status is
            PaymentStatus.Success or
            PaymentStatus.Failed or
            PaymentStatus.Cancelled or
            PaymentStatus.Refunded or
            PaymentStatus.PartiallyRefunded)
        {
            throw new InvalidOperationException(
                "A finalized payment cannot be modified.");
        }
    }

    private static string RequireValue(
        string value,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "Value is required.",
                parameterName);
        }

        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}