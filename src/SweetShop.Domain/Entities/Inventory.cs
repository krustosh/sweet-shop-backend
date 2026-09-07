using SweetShop.Domain.Common;

namespace SweetShop.Domain.Entities;

/// <summary>
/// Represents the current inventory state for a product variant.
/// </summary>
public sealed class Inventory : AuditableEntity
{
    /// <summary>
    /// Gets the identifier of the product variant associated with the inventory.
    /// </summary>
    public Guid ProductVariantId { get; private set; }

    /// <summary>
    /// Gets the quantity currently available for sale.
    /// </summary>
    public decimal AvailableQuantity { get; private set; }

    /// <summary>
    /// Gets the quantity currently reserved for orders.
    /// </summary>
    public decimal ReservedQuantity { get; private set; }

    /// <summary>
    /// Gets the quantity at which a low-stock warning should be triggered.
    /// </summary>
    public decimal LowStockThreshold { get; private set; }

    /// <summary>
    /// Initializes a new inventory record.
    /// </summary>
    /// <param name="productVariantId">
    /// The identifier of the product variant.
    /// </param>
    /// <param name="lowStockThreshold">
    /// The quantity below which the inventory is considered low.
    /// </param>
    public Inventory(
        Guid productVariantId,
        decimal lowStockThreshold)
    {
        if (productVariantId == Guid.Empty)
        {
            throw new ArgumentException(
                "Product variant ID cannot be empty.",
                nameof(productVariantId));
        }

        if (lowStockThreshold < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lowStockThreshold),
                "Low-stock threshold cannot be negative.");
        }

        ProductVariantId = productVariantId;
        LowStockThreshold = lowStockThreshold;
    }

    /// <summary>
    /// Gets the total quantity currently held by the inventory.
    /// </summary>
    public decimal TotalQuantity =>
        AvailableQuantity + ReservedQuantity;

    /// <summary>
    /// Gets a value indicating whether the inventory is at or below
    /// the configured low-stock threshold.
    /// </summary>
    public bool IsLowStock =>
        AvailableQuantity <= LowStockThreshold;

    /// <summary>
    /// Gets a value indicating whether no quantity is currently available.
    /// </summary>
    public bool IsOutOfStock =>
        AvailableQuantity <= 0;

    /// <summary>
    /// Adds quantity to available inventory.
    /// </summary>
    /// <param name="quantity">The quantity to add.</param>
    public void AddStock(decimal quantity)
    {
        ValidatePositiveQuantity(quantity);

        AvailableQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Reserves available inventory for an order.
    /// </summary>
    /// <param name="quantity">The quantity to reserve.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when insufficient available inventory exists.
    /// </exception>
    public void Reserve(decimal quantity)
    {
        ValidatePositiveQuantity(quantity);

        if (quantity > AvailableQuantity)
        {
            throw new InvalidOperationException(
                "Insufficient available inventory.");
        }

        AvailableQuantity -= quantity;
        ReservedQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Releases previously reserved inventory back to available inventory.
    /// </summary>
    /// <param name="quantity">The quantity to release.</param>
    public void ReleaseReservation(decimal quantity)
    {
        ValidatePositiveQuantity(quantity);

        if (quantity > ReservedQuantity)
        {
            throw new InvalidOperationException(
                "Cannot release more inventory than is reserved.");
        }

        ReservedQuantity -= quantity;
        AvailableQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Consumes previously reserved inventory.
    /// </summary>
    /// <param name="quantity">The quantity to consume.</param>
    public void ConsumeReserved(decimal quantity)
    {
        ValidatePositiveQuantity(quantity);

        if (quantity > ReservedQuantity)
        {
            throw new InvalidOperationException(
                "Cannot consume more inventory than is reserved.");
        }

        ReservedQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Removes quantity from available inventory.
    /// </summary>
    /// <param name="quantity">The quantity to remove.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when insufficient available inventory exists.
    /// </exception>
    public void RemoveStock(decimal quantity)
    {
        ValidatePositiveQuantity(quantity);

        if (quantity > AvailableQuantity)
        {
            throw new InvalidOperationException(
                "Insufficient available inventory.");
        }

        AvailableQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the low-stock threshold.
    /// </summary>
    /// <param name="threshold">The new low-stock threshold.</param>
    public void UpdateLowStockThreshold(decimal threshold)
    {
        if (threshold < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(threshold),
                "Low-stock threshold cannot be negative.");
        }

        LowStockThreshold = threshold;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidatePositiveQuantity(decimal quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Inventory quantity must be greater than zero.");
        }
    }
}