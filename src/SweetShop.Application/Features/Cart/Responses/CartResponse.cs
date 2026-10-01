namespace SweetShop.Application.Features.Cart.Responses;

/// <summary>
/// Represents the active shopping cart returned to a customer.
/// </summary>
public sealed record CartResponse(
    Guid Id,
    Guid CustomerId,
    IReadOnlyCollection<CartItemResponse> Items,
    int ItemCount,
    decimal TotalQuantity,
    decimal Subtotal,
    string Currency,
    DateTime CreatedAt,
    DateTime UpdatedAt);
