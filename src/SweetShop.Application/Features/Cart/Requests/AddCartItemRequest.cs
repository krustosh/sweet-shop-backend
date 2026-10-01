namespace SweetShop.Application.Features.Cart.Requests;

/// <summary>
/// Represents a request to add a product variant to the customer's cart.
/// </summary>
/// <param name="ProductVariantId">The product variant identifier.</param>
/// <param name="Quantity">The quantity to add to the cart.</param>
public sealed record AddCartItemRequest(
    Guid ProductVariantId,
    decimal Quantity);
