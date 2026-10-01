namespace SweetShop.Application.Features.Cart.Requests;

/// <summary>
/// Represents a request to change the quantity of an existing cart item.
/// </summary>
/// <param name="Quantity">The replacement quantity.</param>
public sealed record UpdateCartItemRequest(decimal Quantity);
