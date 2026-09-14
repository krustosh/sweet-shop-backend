using SweetShop.Domain.Entities;

using InventoryEntity = SweetShop.Domain.Entities.Inventory;

namespace SweetShop.Application.Features.Inventory;

/// <summary>
/// Defines persistence operations for inventory.
/// </summary>
public interface IInventoryStore
{
    /// <summary>
    /// Gets all inventory records belonging to the specified shop.
    /// </summary>
    Task<IReadOnlyCollection<InventoryEntity>> GetByShopIdAsync(
        Guid shopId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets inventory for a product variant belonging to the specified shop.
    /// </summary>
    Task<InventoryEntity?> GetByVariantIdAsync(
        Guid shopId,
        Guid productVariantId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Determines whether a product variant belongs to the specified shop.
    /// </summary>
    Task<bool> VariantExistsForShopAsync(
        Guid shopId,
        Guid productVariantId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adds an inventory record.
    /// </summary>
    void Add(InventoryEntity inventory);
}