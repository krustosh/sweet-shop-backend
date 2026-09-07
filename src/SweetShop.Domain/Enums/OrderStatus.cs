namespace SweetShop.Domain.Enums;

/// <summary>
/// Defines the lifecycle status of an order.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// The order has been placed by the customer.
    /// </summary>
    Placed = 1,

    /// <summary>
    /// The shop has accepted the order.
    /// </summary>
    Accepted = 2,

    /// <summary>
    /// The order is being prepared.
    /// </summary>
    Preparing = 3,

    /// <summary>
    /// The order is ready for pickup or delivery.
    /// </summary>
    Ready = 4,

    /// <summary>
    /// The order is currently out for delivery.
    /// </summary>
    OutForDelivery = 5,

    /// <summary>
    /// The order has been delivered.
    /// </summary>
    Delivered = 6,

    /// <summary>
    /// The order has been picked up by the customer.
    /// </summary>
    PickedUp = 7,

    /// <summary>
    /// The order has been completed.
    /// </summary>
    Completed = 8,

    /// <summary>
    /// The order has been cancelled.
    /// </summary>
    Cancelled = 9
}