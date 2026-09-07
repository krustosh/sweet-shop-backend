using SweetShop.Domain.Common;

namespace SweetShop.Domain.Entities;

/// <summary>
/// Represents a product variant selected in a shopping cart.
/// </summary>
public sealed class CartItem : Entity
{
    /// <summary>
    /// Gets the identifier of the cart containing the item.
    /// </summary>
    public Guid CartId { get; private set; }

    /// <summary>
    /// Gets the identifier of the selected product variant.
    /// </summary>
    public Guid ProductVariantId { get; private set; }

    /// <summary>
    /// Gets the requested quantity.
    /// </summary>
    public decimal Quantity { get; private set; }

    /// <summary>
    /// Gets the date and time when the cart item was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the cart item was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Initializes a new cart item.
    /// </summary>
    /// <param name="cartId">The identifier of the containing cart.</param>
    /// <param name="productVariantId">The selected product variant.</param>
    /// <param name="quantity">The requested quantity.</param>
    public CartItem(
        Guid cartId,
        Guid productVariantId,
        decimal quantity)
    {
        if (cartId == Guid.Empty)
        {
            throw new ArgumentException(
                "Cart ID cannot be empty.",
                nameof(cartId));
        }

        if (productVariantId == Guid.Empty)
        {
            throw new ArgumentException(
                "Product variant ID cannot be empty.",
                nameof(productVariantId));
        }

        ValidateQuantity(quantity);

        CartId = cartId;
        ProductVariantId = productVariantId;
        Quantity = quantity;

        var now = DateTime.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    /// <summary>
    /// Changes the requested quantity.
    /// </summary>
    /// <param name="quantity">The new requested quantity.</param>
    public void ChangeQuantity(decimal quantity)
    {
        ValidateQuantity(quantity);

        Quantity = quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateQuantity(decimal quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Cart item quantity must be greater than zero.");
        }
    }
}