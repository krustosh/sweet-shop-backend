using SweetShop.Application.Features.Inventory.Requests;
using SweetShop.Application.Features.Inventory.Responses;
using SweetShop.Application.Interfaces;
using SweetShop.Domain.Entities;

using InventoryEntity = SweetShop.Domain.Entities.Inventory;

namespace SweetShop.Application.Features.Inventory;

/// <summary>
/// Provides inventory business operations.
/// </summary>
public sealed class InventoryService : IInventoryService
{
    private readonly IInventoryStore inventoryStore;
    private readonly IInventoryTransactionStore transactionStore;
    private readonly IShopContext shopContext;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryService"/> class.
    /// </summary>
    public InventoryService(
        IInventoryStore inventoryStore,
        IInventoryTransactionStore transactionStore,
        IShopContext shopContext,
        IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(inventoryStore);
        ArgumentNullException.ThrowIfNull(transactionStore);
        ArgumentNullException.ThrowIfNull(shopContext);
        ArgumentNullException.ThrowIfNull(unitOfWork);

        this.inventoryStore = inventoryStore;
        this.transactionStore = transactionStore;
        this.shopContext = shopContext;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<InventoryResponse>> GetInventoryAsync(
        CancellationToken cancellationToken)
    {
        var inventory = await inventoryStore.GetByShopIdAsync(
            shopContext.ShopId,
            cancellationToken);

        return inventory
            .Select(Map)
            .ToArray();
    }

    /// <inheritdoc />
    public async Task<InventoryResponse?> GetInventoryByVariantIdAsync(
        Guid productVariantId,
        CancellationToken cancellationToken)
    {
        var inventory = await inventoryStore.GetByVariantIdAsync(
            shopContext.ShopId,
            productVariantId,
            cancellationToken);

        return inventory is null
            ? null
            : Map(inventory);
    }

    /// <inheritdoc />
    public async Task<InventoryResponse> CreateInventoryAsync(
        Guid productVariantId,
        CreateInventoryRequest request,
        CancellationToken cancellationToken)
    {
        await EnsureVariantExistsAsync(
            productVariantId,
            cancellationToken);

        var existingInventory =
            await inventoryStore.GetByVariantIdAsync(
                shopContext.ShopId,
                productVariantId,
                cancellationToken);

        if (existingInventory is not null)
        {
            throw new InvalidOperationException(
                "Inventory already exists for the specified product variant.");
        }

        var inventory = new InventoryEntity(
            productVariantId,
            request.LowStockThreshold);

        inventoryStore.Add(inventory);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(inventory);
    }

    /// <inheritdoc />
    public async Task<InventoryResponse?> UpdateInventoryThresholdAsync(
        Guid productVariantId,
        decimal lowStockThreshold,
        CancellationToken cancellationToken)
    {
        var inventory = await inventoryStore.GetByVariantIdAsync(
            shopContext.ShopId,
            productVariantId,
            cancellationToken);

        if (inventory is null)
        {
            return null;
        }

        inventory.UpdateLowStockThreshold(lowStockThreshold);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(inventory);
    }

    /// <inheritdoc />
    public async Task<InventoryResponse?> AddStockAsync(
        Guid productVariantId,
        CreateInventoryTransactionRequest request,
        Guid createdBy,
        CancellationToken cancellationToken)
    {
        ValidateCreatedBy(createdBy);

        var inventory = await inventoryStore.GetByVariantIdAsync(
            shopContext.ShopId,
            productVariantId,
            cancellationToken);

        if (inventory is null)
        {
            return null;
        }

        var previousQuantity =
            inventory.AvailableQuantity;

        inventory.AddStock(request.Quantity);

        var transaction = new InventoryTransaction(
            inventory.Id,
            request.Type,
            request.Quantity,
            previousQuantity,
            inventory.AvailableQuantity,
            createdBy,
            request.ReferenceType,
            request.ReferenceId,
            request.Reason);

        transactionStore.Add(transaction);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(inventory);
    }

    /// <inheritdoc />
    public async Task<InventoryResponse?> RemoveStockAsync(
        Guid productVariantId,
        CreateInventoryTransactionRequest request,
        Guid createdBy,
        CancellationToken cancellationToken)
    {
        ValidateCreatedBy(createdBy);

        var inventory = await inventoryStore.GetByVariantIdAsync(
            shopContext.ShopId,
            productVariantId,
            cancellationToken);

        if (inventory is null)
        {
            return null;
        }

        var previousQuantity =
            inventory.AvailableQuantity;

        inventory.RemoveStock(request.Quantity);

        var transaction = new InventoryTransaction(
            inventory.Id,
            request.Type,
            -request.Quantity,
            previousQuantity,
            inventory.AvailableQuantity,
            createdBy,
            request.ReferenceType,
            request.ReferenceId,
            request.Reason);

        transactionStore.Add(transaction);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(inventory);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<InventoryTransactionResponse>>
        GetTransactionsAsync(
            Guid productVariantId,
            CancellationToken cancellationToken)
    {
        var inventory = await inventoryStore.GetByVariantIdAsync(
            shopContext.ShopId,
            productVariantId,
            cancellationToken);

        if (inventory is null)
        {
            return Array.Empty<InventoryTransactionResponse>();
        }

        var transactions =
            await transactionStore.GetByInventoryIdAsync(
                inventory.Id,
                cancellationToken);

        return transactions
            .Select(Map)
            .ToArray();
    }

    private async Task EnsureVariantExistsAsync(
        Guid productVariantId,
        CancellationToken cancellationToken)
    {
        if (productVariantId == Guid.Empty)
        {
            throw new ArgumentException(
                "Product variant ID is required.",
                nameof(productVariantId));
        }

        var exists =
            await inventoryStore.VariantExistsForShopAsync(
                shopContext.ShopId,
                productVariantId,
                cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException(
                "The specified product variant does not exist.");
        }
    }

    private static void ValidateCreatedBy(
        Guid createdBy)
    {
        if (createdBy == Guid.Empty)
        {
            throw new ArgumentException(
                "Created-by user ID is required.",
                nameof(createdBy));
        }
    }

    private static InventoryResponse Map(
        InventoryEntity inventory)
    {
        return new InventoryResponse(
            inventory.Id,
            inventory.ProductVariantId,
            inventory.AvailableQuantity,
            inventory.ReservedQuantity,
            inventory.TotalQuantity,
            inventory.LowStockThreshold,
            inventory.IsLowStock,
            inventory.IsOutOfStock,
            inventory.CreatedAt,
            inventory.UpdatedAt);
    }

    private static InventoryTransactionResponse Map(
        InventoryTransaction transaction)
    {
        return new InventoryTransactionResponse(
            transaction.Id,
            transaction.InventoryId,
            transaction.Type,
            transaction.Quantity,
            transaction.PreviousQuantity,
            transaction.NewQuantity,
            transaction.ReferenceType,
            transaction.ReferenceId,
            transaction.Reason,
            transaction.CreatedBy,
            transaction.CreatedAt);
    }
}