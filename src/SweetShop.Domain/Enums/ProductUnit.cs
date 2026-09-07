namespace SweetShop.Domain.Enums;

/// <summary>
/// Defines the units in which a product variant can be sold.
/// </summary>
public enum ProductUnit
{
    /// <summary>
    /// Product sold by weight in kilograms.
    /// </summary>
    Kilogram = 1,

    /// <summary>
    /// Product sold by individual pieces.
    /// </summary>
    Piece = 2
}