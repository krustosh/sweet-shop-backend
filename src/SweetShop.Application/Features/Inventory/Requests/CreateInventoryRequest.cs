namespace SweetShop.Application.Features.Inventory.Requests;

/// <summary>
/// 
/// </summary>
/// <param name="LowStockThreshold"></param>
public sealed record CreateInventoryRequest(
    decimal LowStockThreshold);