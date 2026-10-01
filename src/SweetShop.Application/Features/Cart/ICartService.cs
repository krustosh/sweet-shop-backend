using SweetShop.Application.Features.Cart.Requests;
using SweetShop.Application.Features.Cart.Responses;

namespace SweetShop.Application.Features.Cart;

/// <summary>
/// Defines customer cart management operations.
/// </summary>
public interface ICartService
{
    /// <summary>
    /// Gets the active cart of the authenticated customer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The authenticated customer's cart.</returns>
    Task<CartResponse> GetMyCartAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Adds a product variant to the authenticated customer's cart.
    /// </summary>
    /// <param name="request">The add-item request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated cart.</returns>
    Task<CartResponse> AddItemAsync(
        AddCartItemRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Changes the quantity of an existing cart item.
    /// </summary>
    /// <param name="cartItemId">The cart item identifier.</param>
    /// <param name="request">The quantity update request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated cart, or <see langword="null"/> when the item does not belong to the customer.</returns>
    Task<CartResponse?> UpdateItemAsync(
        Guid cartItemId,
        UpdateCartItemRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Removes an item from the authenticated customer's cart.
    /// </summary>
    /// <param name="cartItemId">The cart item identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><see langword="true"/> when the item was removed; otherwise, <see langword="false"/>.</returns>
    Task<bool> RemoveItemAsync(
        Guid cartItemId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Removes all items from the authenticated customer's cart.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><see langword="true"/> when a cart existed; otherwise, <see langword="false"/>.</returns>
    Task<bool> ClearAsync(CancellationToken cancellationToken);
}
