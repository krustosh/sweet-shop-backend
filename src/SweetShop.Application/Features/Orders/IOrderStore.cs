using CartEntity = SweetShop.Domain.Entities.Cart;
using InventoryEntity = SweetShop.Domain.Entities.Inventory;
using SweetShop.Domain.Entities;

namespace SweetShop.Application.Features.Orders;

/// <summary>
/// Defines persistence operations required by customer order management.
/// </summary>
public interface IOrderStore
{
    /// <summary>
    /// Gets all orders belonging to a customer.
    /// </summary>
    /// <param name="customerId">The customer identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The customer's orders.</returns>
    Task<IReadOnlyCollection<Order>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets a specific order belonging to a customer.
    /// </summary>
    /// <param name="customerId">The customer identifier.</param>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The order when found; otherwise, <see langword="null"/>.</returns>
    Task<Order?> GetByCustomerAndIdAsync(
        Guid customerId,
        Guid orderId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets the authenticated customer's active cart including its items.
    /// </summary>
    /// <param name="customerId">The customer identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The active cart when found; otherwise, <see langword="null"/>.</returns>
    Task<CartEntity?> GetCartByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets the current active product variant information for an order item.
    /// </summary>
    /// <param name="shopId">The current shop identifier.</param>
    /// <param name="productVariantId">The product variant identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The current variant information when available.</returns>
    Task<OrderItemDetails?> GetActiveVariantDetailsAsync(
        Guid shopId,
        Guid productVariantId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets inventory for a product variant belonging to the current shop.
    /// </summary>
    /// <param name="shopId">The current shop identifier.</param>
    /// <param name="productVariantId">The product variant identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The inventory record when found; otherwise, <see langword="null"/>.</returns>
    Task<InventoryEntity?> GetInventoryByVariantIdAsync(
        Guid shopId,
        Guid productVariantId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets a saved customer address.
    /// </summary>
    /// <param name="addressId">The address identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The address when found; otherwise, <see langword="null"/>.</returns>
    Task<Address?> GetAddressByIdAsync(
        Guid addressId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adds an order to the data store.
    /// </summary>
    /// <param name="order">The order to add.</param>
    void Add(Order order);

    /// <summary>
    /// Adds an order item to the data store.
    /// </summary>
    /// <param name="item">The order item to add.</param>
    void AddItem(OrderItem item);

    /// <summary>
    /// Adds an inventory transaction to the data store.
    /// </summary>
    /// <param name="transaction">The inventory transaction to add.</param>
    void AddInventoryTransaction(InventoryTransaction transaction);
}