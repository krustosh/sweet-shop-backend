using SweetShop.Domain.Common;
using SweetShop.Domain.Enums;
using SweetShop.Domain.ValueObjects;

namespace SweetShop.Domain.Entities;

/// <summary>
/// Represents an item captured within an order.
/// </summary>
public sealed class OrderItem : Entity
{
    /// <summary>
    /// Gets the identifier of the order containing this item.
    /// </summary>
    public Guid OrderId { get; private set; }

    /// <summary>
    /// Gets the identifier of the product at the time of ordering.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Gets the identifier of the product variant at the time of ordering.
    /// </summary>
    public Guid ProductVariantId { get; private set; }

    /// <summary>
    /// Gets the product name captured at the time of ordering.
    /// </summary>
    public string ProductName { get; private set; }

    /// <summary>
    /// Gets the variant name captured at the time of ordering.
    /// </summary>
    public string VariantName { get; private set; }

    /// <summary>
    /// Gets the quantity ordered.
    /// </summary>
    public decimal Quantity { get; private set; }

    /// <summary>
    /// Gets the selling unit of the ordered variant.
    /// </summary>
    public ProductUnit Unit { get; private set; }

    /// <summary>
    /// Gets the unit price captured at the time of ordering.
    /// </summary>
    public Money UnitPrice { get; private set; }

    /// <summary>
    /// Gets the calculated line total.
    /// </summary>
    public Money LineTotal { get; private set; }

    /// <summary>
    /// Gets the date and time when the order item was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    private OrderItem()
    {
        ProductName = string.Empty;
        VariantName = string.Empty;
        Unit = ProductUnit.Piece;
        UnitPrice = new Money(0, DomainConstants.CurrencyInr);
        LineTotal = new Money(0, DomainConstants.CurrencyInr);
    }

    /// <summary>
    /// Initializes a new order item.
    /// </summary>
    /// <param name="orderId">The identifier of the containing order.</param>
    /// <param name="productId">The product identifier.</param>
    /// <param name="productVariantId">The product variant identifier.</param>
    /// <param name="productName">The product name snapshot.</param>
    /// <param name="variantName">The variant name snapshot.</param>
    /// <param name="quantity">The quantity ordered.</param>
    /// <param name="unit">The selling unit.</param>
    /// <param name="unitPrice">The unit price.</param>
    public OrderItem(
        Guid orderId,
        Guid productId,
        Guid productVariantId,
        string productName,
        string variantName,
        decimal quantity,
        ProductUnit unit,
        Money unitPrice)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException(
                "Order ID cannot be empty.",
                nameof(orderId));
        }

        if (productId == Guid.Empty)
        {
            throw new ArgumentException(
                "Product ID cannot be empty.",
                nameof(productId));
        }

        if (productVariantId == Guid.Empty)
        {
            throw new ArgumentException(
                "Product variant ID cannot be empty.",
                nameof(productVariantId));
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Order item quantity must be greater than zero.");
        }

        ArgumentNullException.ThrowIfNull(unitPrice);

        OrderId = orderId;
        ProductId = productId;
        ProductVariantId = productVariantId;
        ProductName = RequireValue(productName, nameof(productName));
        VariantName = RequireValue(variantName, nameof(variantName));
        Quantity = quantity;
        Unit = unit;
        UnitPrice = unitPrice;
        LineTotal = CalculateLineTotal(quantity, unitPrice);
        CreatedAt = DateTime.UtcNow;
    }

    private static Money CalculateLineTotal(
        decimal quantity,
        Money unitPrice)
    {
        return new Money(
            quantity * unitPrice.Amount,
            unitPrice.Currency);
    }

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
}