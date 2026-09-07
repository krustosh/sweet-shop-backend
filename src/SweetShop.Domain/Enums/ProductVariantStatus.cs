namespace SweetShop.Domain.Enums;

/// <summary>
/// Defines the lifecycle status of a product variant.
/// </summary>
public enum ProductVariantStatus
{
    /// <summary>
    /// The variant is active and available for normal catalog operations.
    /// </summary>
    Active = 1,

    /// <summary>
    /// The variant is inactive and unavailable for normal catalog operations.
    /// </summary>
    Inactive = 2
}