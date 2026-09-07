using SweetShop.Domain.Common;
using SweetShop.Domain.Enums;

namespace SweetShop.Domain.Entities;

/// <summary>
/// Represents a sweet product in the shop catalog.
/// </summary>
public sealed class Product : AuditableEntity
{
    /// <summary>
    /// Gets the identifier of the shop that owns the product.
    /// </summary>
    public Guid ShopId { get; private set; }

    /// <summary>
    /// Gets the identifier of the category that contains the product.
    /// </summary>
    public Guid CategoryId { get; private set; }

    /// <summary>
    /// Gets the product name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the product description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the product display order.
    /// </summary>
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// Gets the product status.
    /// </summary>
    public ProductStatus Status { get; private set; }

    /// <summary>
    /// Initializes a new product.
    /// </summary>
    /// <param name="shopId">The identifier of the owning shop.</param>
    /// <param name="categoryId">The identifier of the product category.</param>
    /// <param name="name">The product name.</param>
    /// <param name="displayOrder">The product display order.</param>
    public Product(
        Guid shopId,
        Guid categoryId,
        string name,
        int displayOrder)
    {
        if (shopId == Guid.Empty)
        {
            throw new ArgumentException(
                "Shop ID cannot be empty.",
                nameof(shopId));
        }

        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException(
                "Category ID cannot be empty.",
                nameof(categoryId));
        }

        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        ShopId = shopId;
        CategoryId = categoryId;
        Name = RequireValue(name, nameof(name));
        DisplayOrder = displayOrder;
        Status = ProductStatus.Active;
    }

    /// <summary>
    /// Updates the product information.
    /// </summary>
    /// <param name="name">The product name.</param>
    /// <param name="description">The product description.</param>
    /// <param name="displayOrder">The product display order.</param>
    public void UpdateInformation(
        string name,
        string? description,
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
        DisplayOrder = displayOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Changes the category assigned to the product.
    /// </summary>
    /// <param name="categoryId">The identifier of the new category.</param>
    public void ChangeCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException(
                "Category ID cannot be empty.",
                nameof(categoryId));
        }

        CategoryId = categoryId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the product.
    /// </summary>
    public void Activate()
    {
        Status = ProductStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the product.
    /// </summary>
    public void Deactivate()
    {
        Status = ProductStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string RequireValue(string value, string parameterName)
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