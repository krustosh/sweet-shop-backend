using SweetShop.Domain.Common;
using SweetShop.Domain.Enums;

namespace SweetShop.Domain.Entities;

/// <summary>
/// Represents a product category within a shop.
/// </summary>
public sealed class Category : AuditableEntity
{
    /// <summary>
    /// Gets the identifier of the shop that owns the category.
    /// </summary>
    public Guid ShopId { get; private set; }

    /// <summary>
    /// Gets the category name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the category description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the category image URL.
    /// </summary>
    public string? ImageUrl { get; private set; }

    /// <summary>
    /// Gets the category display order.
    /// </summary>
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// Gets the category status.
    /// </summary>
    public CategoryStatus Status { get; private set; }

    /// <summary>
    /// Initializes a new category.
    /// </summary>
    /// <param name="shopId">The identifier of the owning shop.</param>
    /// <param name="name">The category name.</param>
    /// <param name="description">The category description.</param>
    /// <param name="imageUrl">The category image URL.</param>
    /// <param name="displayOrder">The category display order.</param>
    public Category(
        Guid shopId,
        string name,
        string? description,
        string? imageUrl,
        int displayOrder)
    {
        if (shopId == Guid.Empty)
        {
            throw new ArgumentException(
                "Shop ID cannot be empty.",
                nameof(shopId));
        }

        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        ShopId = shopId;
        Name = RequireValue(name, nameof(name));
        Description = NormalizeOptional(description);
        ImageUrl = NormalizeOptional(imageUrl);
        DisplayOrder = displayOrder;
        Status = CategoryStatus.Active;
    }

    /// <summary>
    /// Updates the category information.
    /// </summary>
    /// <param name="name">The category name.</param>
    /// <param name="description">The category description.</param>
    /// <param name="imageUrl">The category image URL.</param>
    /// <param name="displayOrder">The category display order.</param>
    public void UpdateInformation(
        string name,
        string? description,
        string? imageUrl,
        int displayOrder)
    {
        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        Name = RequireValue(name, nameof(name));
        Description = NormalizeOptional(description);
        ImageUrl = NormalizeOptional(imageUrl);
        DisplayOrder = displayOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the category.
    /// </summary>
    public void Activate()
    {
        Status = CategoryStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the category.
    /// </summary>
    public void Deactivate()
    {
        Status = CategoryStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Validates and normalizes a required string value.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="parameterName">The parameter name.</param>
    /// <returns>The trimmed value.</returns>
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

    /// <summary>
    /// Normalizes an optional string value.
    /// </summary>
    /// <param name="value">The optional value.</param>
    /// <returns>
    /// The trimmed value, or <see langword="null"/> when empty.
    /// </returns>
    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}