using Microsoft.EntityFrameworkCore;
using SweetShop.Application.Features.Inventory;
using SweetShop.Domain.Entities;
using SweetShop.Infrastructure.Persistence.Context;

namespace SweetShop.Infrastructure.Persistence.Stores;

/// <summary>
/// Provides persistence operations for inventory transactions.
/// </summary>
public sealed class InventoryTransactionStore
    : IInventoryTransactionStore
{
    private readonly SweetShopDbContext dbContext;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="InventoryTransactionStore"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public InventoryTransactionStore(
        SweetShopDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        this.dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<InventoryTransaction>>
        GetByInventoryIdAsync(
            Guid inventoryId,
            CancellationToken cancellationToken)
    {
        if (inventoryId == Guid.Empty)
        {
            throw new ArgumentException(
                "Inventory ID is required.",
                nameof(inventoryId));
        }

        return await dbContext.Set<InventoryTransaction>()
            .Where(transaction =>
                transaction.InventoryId == inventoryId)
            .OrderByDescending(transaction =>
                transaction.CreatedAt)
            .ThenByDescending(transaction =>
                transaction.Id)
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Add(InventoryTransaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        dbContext.Set<InventoryTransaction>()
            .Add(transaction);
    }
}