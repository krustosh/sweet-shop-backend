using SweetShop.Domain.Entities;

namespace SweetShop.Application.Features.Inventory;

/// <summary>
/// Defines persistence operations for inventory transactions.
/// </summary>
public interface IInventoryTransactionStore
{
    /// <summary>
    /// Gets transactions for an inventory record.
    /// </summary>
    Task<IReadOnlyCollection<InventoryTransaction>> GetByInventoryIdAsync(
        Guid inventoryId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adds an inventory transaction.
    /// </summary>
    void Add(InventoryTransaction transaction);
}