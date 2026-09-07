using SweetShop.Domain.Common;
using SweetShop.Domain.Enums;
using SweetShop.Domain.ValueObjects;

namespace SweetShop.Domain.Entities;

/// <summary>
/// Represents a purchasable variant of a sweet product.
/// </summary>
public sealed class ProductVariant : AuditableEntity
{
    /// <summary>
    /// Gets the identifier of the product that owns the variant.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Gets the variant name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the fixed selling quantity and unit.
    /// </summary>
    public ProductQuantity Quantity { get; private set; }

    /// <summary>
    /// Gets the selling price.
    /// </summary>
    public Money Price { get; private set; }

    /// <summary>
    /// Gets the variant display order.
    /// </summary>
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// Gets the variant status.
    /// </summary>
    public ProductVariantStatus Status { get; private set; }

    /// <summary>
    /// Initializes a new product variant.
    /// </summary>
    /// <param name="productId">The identifier of the owning product.</param>
    /// <param name="name">The variant name.</param>
    /// <param name="quantity">The fixed selling quantity and unit.</param>
    /// <param name="price">The selling price.</param>
    /// <param name="displayOrder">The variant display order.</param>
    public ProductVariant(
        Guid productId,
        string name,
        ProductQuantity quantity,
        Money price,
        int displayOrder)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException(
                "Product ID cannot be empty.",
                nameof(productId));
        }
            ArgumentNullException.ThrowIfNull(quantity);
            ArgumentNullException.ThrowIfNull(price);
        
        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        ProductId = productId;
        Name = RequireValue(name, nameof(name));
        Quantity = quantity;
        Price = price;
        DisplayOrder = displayOrder;
        Status = ProductVariantStatus.Active;
    }

    /// <summary>
    /// Updates mutable variant information.
    /// </summary>
    /// <param name="name">The variant name.</param>
    /// <param name="price">The selling price.</param>
    /// <param name="displayOrder">The variant display order.</param>
    public void UpdateInformation(
        string name,
        Money price,
        int displayOrder)
    {

        ArgumentNullException.ThrowIfNull(price);
        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        Name = RequireValue(name, nameof(name));
        Price = price;
        DisplayOrder = displayOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the product variant.
    /// </summary>
    public void Activate()
    {
        Status = ProductVariantStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the product variant.
    /// </summary>
    public void Deactivate()
    {
        Status = ProductVariantStatus.Inactive;
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
}