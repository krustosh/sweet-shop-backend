using SweetShop.Domain.Enums;

namespace SweetShop.Domain.ValueObjects;

/// <summary>
/// Represents the fixed selling quantity and unit of a product variant.
/// </summary>
public sealed record ProductQuantity
{
    /// <summary>
    /// Gets the quantity value.
    /// </summary>
    public decimal Value { get; }

    /// <summary>
    /// Gets the unit in which the quantity is sold.
    /// </summary>
    public ProductUnit Unit { get; }

    /// <summary>
    /// Initializes a new product quantity.
    /// </summary>
    /// <param name="value">The quantity value.</param>
    /// <param name="unit">The selling unit.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the quantity is zero or negative.
    /// </exception>
    public ProductQuantity(decimal value, ProductUnit unit)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                "Product quantity must be greater than zero.");
        }

        Value = value;
        Unit = unit;
    }
}