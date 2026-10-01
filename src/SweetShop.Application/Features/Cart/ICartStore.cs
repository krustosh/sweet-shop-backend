using CartEntity = SweetShop.Domain.Entities.Cart;
using SweetShop.Domain.Entities;

namespace SweetShop.Application.Features.Cart;

/// <summary>
/// Defines persistence operations required by the customer cart service.
/// </summary>
public interface ICartStore
{
    /// <summary>
    /// Gets the cart belonging to the specified customer, including its items.
    /// </summary>
    /// <param name="customerId">The customer identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The customer's cart if one exists; otherwise, <see langword="null"/>.
    /// </returns>
    Task<CartEntity?> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets a cart item belonging to the specified cart.
    /// </summary>
    /// <param name="cartId">The cart identifier.</param>
    /// <param name="cartItemId">The cart item identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The cart item if it belongs to the cart and exists; otherwise,
    /// <see langword="null"/>.
    /// </returns>
    Task<CartItem?> GetItemByIdAsync(
        Guid cartId,
        Guid cartItemId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets an active product variant and its product information for the
    /// current shop.
    /// </summary>
    /// <param name="shopId">The shop identifier.</param>
    /// <param name="productVariantId">The product variant identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The active variant details if available; otherwise,
    /// <see langword="null"/>.
    /// </returns>
    Task<CartVariantDetails?> GetActiveVariantDetailsAsync(
        Guid shopId,
        Guid productVariantId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new cart to the data store.
    /// </summary>
    /// <param name="cart">The cart to add.</param>
    void Add(CartEntity cart);

    /// <summary>
    /// Adds a new cart item to the data store.
    /// </summary>
    /// <param name="item">The cart item to add.</param>
    void AddItem(CartItem item);
}