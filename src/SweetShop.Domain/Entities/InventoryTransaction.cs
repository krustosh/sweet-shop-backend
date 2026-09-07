using SweetShop.Domain.Common;
using SweetShop.Domain.Enums;

namespace SweetShop.Domain.Entities;

/// <summary>
/// Represents an auditable change to inventory.
/// </summary>
public sealed class InventoryTransaction : Entity
{
    /// <summary>
    /// Gets the identifier of the inventory record affected by the transaction.
    /// </summary>
    public Guid InventoryId { get; private set; }

    /// <summary>
    /// Gets the type of inventory transaction.
    /// </summary>
    public InventoryTransactionType Type { get; private set; }

    /// <summary>
    /// Gets the signed quantity change.
    /// </summary>
    public decimal Quantity { get; private set; }

    /// <summary>
    /// Gets the inventory quantity before the transaction.
    /// </summary>
    public decimal PreviousQuantity { get; private set; }

    /// <summary>
    /// Gets the inventory quantity after the transaction.
    /// </summary>
    public decimal NewQuantity { get; private set; }

    /// <summary>
    /// Gets the type of entity that caused the transaction.
    /// </summary>
    public string? ReferenceType { get; private set; }

    /// <summary>
    /// Gets the identifier of the entity that caused the transaction.
    /// </summary>
    public Guid? ReferenceId { get; private set; }

    /// <summary>
    /// Gets the reason for the inventory change.
    /// </summary>
    public string? Reason { get; private set; }

    /// <summary>
    /// Gets the identifier of the user who created the transaction.
    /// </summary>
    public Guid CreatedBy { get; private set; }

    /// <summary>
    /// Gets the date and time when the transaction was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Initializes a new inventory transaction.
    /// </summary>
    /// <param name="inventoryId">The affected inventory identifier.</param>
    /// <param name="type">The transaction type.</param>
    /// <param name="quantity">The signed quantity change.</param>
    /// <param name="previousQuantity">The quantity before the transaction.</param>
    /// <param name="newQuantity">The quantity after the transaction.</param>
    /// <param name="createdBy">The user responsible for the transaction.</param>
    /// <param name="referenceType">The optional reference entity type.</param>
    /// <param name="referenceId">The optional reference entity identifier.</param>
    /// <param name="reason">The optional reason for the change.</param>
    public InventoryTransaction(
        Guid inventoryId,
        InventoryTransactionType type,
        decimal quantity,
        decimal previousQuantity,
        decimal newQuantity,
        Guid createdBy,
        string? referenceType = null,
        Guid? referenceId = null,
        string? reason = null)
    {
        if (inventoryId == Guid.Empty)
        {
            throw new ArgumentException(
                "Inventory ID cannot be empty.",
                nameof(inventoryId));
        }

        if (quantity == 0)
        {
            throw new ArgumentException(
                "Transaction quantity cannot be zero.",
                nameof(quantity));
        }

        if (previousQuantity < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(previousQuantity),
                "Previous quantity cannot be negative.");
        }

        if (newQuantity < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(newQuantity),
                "New quantity cannot be negative.");
        }

        if (createdBy == Guid.Empty)
        {
            throw new ArgumentException(
                "Created-by user ID cannot be empty.",
                nameof(createdBy));
        }

        InventoryId = inventoryId;
        Type = type;
        Quantity = quantity;
        PreviousQuantity = previousQuantity;
        NewQuantity = newQuantity;
        CreatedBy = createdBy;
        ReferenceType = NormalizeOptional(referenceType);
        ReferenceId = referenceId;
        Reason = NormalizeOptional(reason);
        CreatedAt = DateTime.UtcNow;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}