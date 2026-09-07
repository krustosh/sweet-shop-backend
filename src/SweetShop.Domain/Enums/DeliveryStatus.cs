namespace SweetShop.Domain.Enums;

/// <summary>
/// Defines the lifecycle status of a delivery.
/// </summary>
public enum DeliveryStatus
{
    /// <summary>
    /// Delivery is awaiting assignment.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Delivery has been assigned to a delivery person.
    /// </summary>
    Assigned = 2,

    /// <summary>
    /// Delivery is currently in progress.
    /// </summary>
    OutForDelivery = 3,

    /// <summary>
    /// Delivery was successfully completed.
    /// </summary>
    Delivered = 4,

    /// <summary>
    /// Delivery could not be completed.
    /// </summary>
    Failed = 5,

    /// <summary>
    /// Delivery was cancelled.
    /// </summary>
    Cancelled = 6
}