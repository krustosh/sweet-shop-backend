namespace SweetShop.Domain.Enums;

/// <summary>
/// Defines the lifecycle status of a shop.
/// </summary>
public enum ShopStatus
{
    /// <summary>
    /// The shop is active and available for operations.
    /// </summary>
    Active = 1,

    /// <summary>
    /// The shop is inactive and unavailable for operations.
    /// </summary>
    Inactive = 2
}