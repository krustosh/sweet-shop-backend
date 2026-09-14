namespace SweetShop.Application.Features.Inventory.Responses;

/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="ProductVariantId"></param>
/// <param name="AvailableQuantity"></param>
/// <param name="ReservedQuantity"></param>
/// <param name="TotalQuantity"></param>
/// <param name="LowStockThreshold"></param>
/// <param name="IsLowStock"></param>
/// <param name="IsOutOfStock"></param>
/// <param name="CreatedAt"></param>
/// <param name="UpdatedAt"></param>
public sealed record InventoryResponse(
    Guid Id,
    Guid ProductVariantId,
    decimal AvailableQuantity,
    decimal ReservedQuantity,
    decimal TotalQuantity,
    decimal LowStockThreshold,
    bool IsLowStock,
    bool IsOutOfStock,
    DateTime CreatedAt,
    DateTime UpdatedAt);