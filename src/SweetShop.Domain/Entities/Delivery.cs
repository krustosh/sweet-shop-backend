using SweetShop.Domain.Common;
using SweetShop.Domain.Enums;

namespace SweetShop.Domain.Entities;

/// <summary>
/// Represents the delivery fulfillment record for an order.
/// </summary>
public sealed class Delivery : AuditableEntity
{
    /// <summary>
    /// Gets the identifier of the associated order.
    /// </summary>
    public Guid OrderId { get; private set; }

    /// <summary>
    /// Gets the identifier of the assigned delivery person.
    /// </summary>
    public Guid? DeliveryPersonId { get; private set; }

    /// <summary>
    /// Gets the current delivery status.
    /// </summary>
    public DeliveryStatus Status { get; private set; }

    /// <summary>
    /// Gets the date and time when the delivery was assigned.
    /// </summary>
    public DateTime? AssignedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the order was picked up for delivery.
    /// </summary>
    public DateTime? PickedUpAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the order was delivered.
    /// </summary>
    public DateTime? DeliveredAt { get; private set; }

    /// <summary>
    /// Gets the date and time when delivery failed.
    /// </summary>
    public DateTime? FailedAt { get; private set; }

    /// <summary>
    /// Gets the reason for delivery failure.
    /// </summary>
    public string? FailureReason { get; private set; }

    /// <summary>
    /// Gets an optional delivery note.
    /// </summary>
    public string? DeliveryNote { get; private set; }

    /// <summary>
    /// Initializes a new delivery record.
    /// </summary>
    /// <param name="orderId">The identifier of the associated order.</param>
    public Delivery(Guid orderId)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException(
                "Order ID cannot be empty.",
                nameof(orderId));
        }

        OrderId = orderId;
        Status = DeliveryStatus.Pending;
    }

    /// <summary>
    /// Assigns the delivery to a delivery person.
    /// </summary>
    /// <param name="deliveryPersonId">
    /// The identifier of the delivery person.
    /// </param>
    public void Assign(Guid deliveryPersonId)
    {
        if (deliveryPersonId == Guid.Empty)
        {
            throw new ArgumentException(
                "Delivery person ID cannot be empty.",
                nameof(deliveryPersonId));
        }

        if (Status is DeliveryStatus.Delivered or
            DeliveryStatus.Failed or
            DeliveryStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "A finalized delivery cannot be assigned.");
        }

        DeliveryPersonId = deliveryPersonId;
        Status = DeliveryStatus.Assigned;
        AssignedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the delivery as out for delivery.
    /// </summary>
    public void MarkOutForDelivery()
    {
        if (Status != DeliveryStatus.Assigned)
        {
            throw new InvalidOperationException(
                "Delivery must be assigned before it can go out for delivery.");
        }

        Status = DeliveryStatus.OutForDelivery;
        PickedUpAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the delivery as delivered.
    /// </summary>
    /// <param name="deliveryNote">An optional delivery note.</param>
    public void MarkDelivered(string? deliveryNote)
    {
        if (Status != DeliveryStatus.OutForDelivery)
        {
            throw new InvalidOperationException(
                "Delivery must be out for delivery before it can be delivered.");
        }

        Status = DeliveryStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        DeliveryNote = NormalizeOptional(deliveryNote);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the delivery as failed.
    /// </summary>
    /// <param name="failureReason">The reason delivery failed.</param>
    public void MarkFailed(string failureReason)
    {
        if (Status is DeliveryStatus.Delivered or
            DeliveryStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "A delivered or cancelled delivery cannot be marked as failed.");
        }

        FailureReason = RequireValue(
            failureReason,
            nameof(failureReason));

        Status = DeliveryStatus.Failed;
        FailedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the delivery.
    /// </summary>
    public void Cancel()
    {
        if (Status == DeliveryStatus.Delivered)
        {
            throw new InvalidOperationException(
                "A delivered delivery cannot be cancelled.");
        }

        Status = DeliveryStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
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