namespace SweetShop.Domain.Enums;

/// <summary>
/// Defines how an order is fulfilled.
/// </summary>
public enum FulfillmentType
{
    /// <summary>
    /// The order is delivered to the customer.
    /// </summary>
    Delivery = 1,

    /// <summary>
    /// The customer collects the order from the shop.
    /// </summary>
    Pickup = 2
}
