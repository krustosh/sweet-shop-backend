namespace SweetShop.Domain.Enums;

/// <summary>
/// Defines the lifecycle status of a product category.
/// </summary>
public enum CategoryStatus
{
    /// <summary>
    /// The category is active and visible for normal catalog operations.
    /// </summary>
    Active = 1,

    /// <summary>
    /// The category is inactive and unavailable for normal catalog operations.
    /// </summary>
    Inactive = 2
}