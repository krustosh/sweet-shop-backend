using SweetShop.Domain.Enums;

namespace SweetShop.Application.Features.Inventory.Responses;

/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="InventoryId"></param>
/// <param name="Type"></param>
/// <param name="Quantity"></param>
/// <param name="PreviousQuantity"></param>
/// <param name="NewQuantity"></param>
/// <param name="ReferenceType"></param>
/// <param name="ReferenceId"></param>
/// <param name="Reason"></param>
/// <param name="CreatedBy"></param>
/// <param name="CreatedAt"></param>
public sealed record InventoryTransactionResponse(
    Guid Id,
    Guid InventoryId,
    InventoryTransactionType Type,
    decimal Quantity,
    decimal PreviousQuantity,
    decimal NewQuantity,
    string? ReferenceType,
    Guid? ReferenceId,
    string? Reason,
    Guid CreatedBy,
    DateTime CreatedAt);