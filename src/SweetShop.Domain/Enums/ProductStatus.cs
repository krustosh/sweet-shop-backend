namespace SweetShop.Domain.Enums;

/// <summary>
/// Defines the lifecycle status of a product.
/// </summary>
public enum ProductStatus
{
    /// <summary>
    /// The product is active and available for normal catalog operations.
    /// </summary>
    Active = 1,

    /// <summary>
    /// The product is inactive and unavailable for normal catalog operations.
    /// </summary>
    Inactive = 2
}