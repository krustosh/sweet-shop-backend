using SweetShop.Domain.Enums;

namespace SweetShop.Application.Features.Inventory.Requests;

/// <summary>
/// 
/// </summary>
/// <param name="Quantity"></param>
/// <param name="Type"></param>
/// <param name="ReferenceType"></param>
/// <param name="ReferenceId"></param>
/// <param name="Reason"></param>
public sealed record CreateInventoryTransactionRequest(
    decimal Quantity,
    InventoryTransactionType Type,
    string? ReferenceType,
    Guid? ReferenceId,
    string? Reason);