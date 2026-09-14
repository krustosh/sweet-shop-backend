using SweetShop.Application.Features.Inventory.Requests;
using SweetShop.Application.Features.Inventory.Responses;

namespace SweetShop.Application.Features.Inventory;

/// <summary>
/// Defines inventory business operations.
/// </summary>
public interface IInventoryService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<InventoryResponse>> GetInventoryAsync(
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productVariantId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<InventoryResponse?> GetInventoryByVariantIdAsync(
        Guid productVariantId,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productVariantId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<InventoryResponse> CreateInventoryAsync(
        Guid productVariantId,
        CreateInventoryRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productVariantId"></param>
    /// <param name="lowStockThreshold"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<InventoryResponse?> UpdateInventoryThresholdAsync(
        Guid productVariantId,
        decimal lowStockThreshold,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productVariantId"></param>
    /// <param name="request"></param>
    /// <param name="createdBy"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<InventoryResponse?> AddStockAsync(
        Guid productVariantId,
        CreateInventoryTransactionRequest request,
        Guid createdBy,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productVariantId"></param>
    /// <param name="request"></param>
    /// <param name="createdBy"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<InventoryResponse?> RemoveStockAsync(
        Guid productVariantId,
        CreateInventoryTransactionRequest request,
        Guid createdBy,
        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productVariantId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<InventoryTransactionResponse>>
        GetTransactionsAsync(
            Guid productVariantId,
            CancellationToken cancellationToken);
}