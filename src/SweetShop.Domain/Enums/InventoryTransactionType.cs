namespace SweetShop.Domain.Enums;

/// <summary>
/// Defines the business reason for an inventory transaction.
/// </summary>
public enum InventoryTransactionType
{
    /// <summary>
    /// Inventory added through production.
    /// </summary>
    Production = 1,

    /// <summary>
    /// Inventory added through purchase.
    /// </summary>
    Purchase = 2,

    /// <summary>
    /// Inventory consumed by an order.
    /// </summary>
    Order = 3,

    /// <summary>
    /// Inventory removed because of damage.
    /// </summary>
    Damage = 4,

    /// <summary>
    /// Inventory removed because of wastage.
    /// </summary>
    Wastage = 5,

    /// <summary>
    /// Inventory adjusted manually.
    /// </summary>
    Correction = 6,

    /// <summary>
    /// Inventory returned to stock.
    /// </summary>
    Return = 7,

    /// <summary>
    /// Inventory changed for another business reason.
    /// </summary>
    Other = 8
}