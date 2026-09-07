using SweetShop.Domain.Common;

namespace SweetShop.Domain.Entities;

/// <summary>
/// Represents an image associated with a product.
/// </summary>
public sealed class ProductImage : AuditableEntity
{
    /// <summary>
    /// Gets the identifier of the product associated with the image.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Gets the image URL.
    /// </summary>
    public string Url { get; private set; }

    /// <summary>
    /// Gets the alternative text for the image.
    /// </summary>
    public string? AltText { get; private set; }

    /// <summary>
    /// Gets the display order of the image.
    /// </summary>
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this is the primary product image.
    /// </summary>
    public bool IsPrimary { get; private set; }

    /// <summary>
    /// Initializes a new product image.
    /// </summary>
    /// <param name="productId">The identifier of the associated product.</param>
    /// <param name="url">The image URL.</param>
    /// <param name="displayOrder">The image display order.</param>
    /// <param name="isPrimary">Whether the image is the primary image.</param>
    public ProductImage(
        Guid productId,
        string url,
        int displayOrder,
        bool isPrimary)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException(
                "Product ID cannot be empty.",
                nameof(productId));
        }

        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        ProductId = productId;
        Url = RequireValue(url, nameof(url));
        DisplayOrder = displayOrder;
        IsPrimary = isPrimary;
    }

    /// <summary>
    /// Updates the image information.
    /// </summary>
    /// <param name="url">The image URL.</param>
    /// <param name="altText">The alternative text.</param>
    /// <param name="displayOrder">The image display order.</param>
    public void UpdateInformation(
        string url,
        string? altText,
        int displayOrder)
    {
        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        Url = RequireValue(url, nameof(url));
        AltText = NormalizeOptional(altText);
        DisplayOrder = displayOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the image as the primary product image.
    /// </summary>
    public void MarkAsPrimary()
    {
        IsPrimary = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Removes the primary designation from the image.
    /// </summary>
    public void RemovePrimaryDesignation()
    {
        IsPrimary = false;
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