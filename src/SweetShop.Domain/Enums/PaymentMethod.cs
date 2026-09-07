namespace SweetShop.Domain.Enums;

/// <summary>
/// Defines the payment methods supported by the shop.
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// Payment through UPI.
    /// </summary>
    Upi = 1,

    /// <summary>
    /// Payment using cash.
    /// </summary>
    Cash = 2
}