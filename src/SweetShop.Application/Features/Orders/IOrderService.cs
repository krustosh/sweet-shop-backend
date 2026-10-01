using SweetShop.Application.Features.Orders.Requests;
using SweetShop.Application.Features.Orders.Responses;

namespace SweetShop.Application.Features.Orders;

/// <summary>
/// Defines customer-facing order management operations.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Gets all orders belonging to the authenticated customer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The customer's orders.</returns>
    Task<IReadOnlyCollection<OrderResponse>> GetMyOrdersAsync(
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets an order belonging to the authenticated customer.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The order when found; otherwise, <see langword="null"/>.</returns>
    Task<OrderResponse?> GetMyOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Creates an order from the authenticated customer's active cart.
    /// </summary>
    /// <param name="request">The order creation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The newly created order.</returns>
    Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Cancels an order belonging to the authenticated customer.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="request">The cancellation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The cancelled order when found; otherwise, <see langword="null"/>.</returns>
    Task<OrderResponse?> CancelAsync(
        Guid orderId,
        CancelOrderRequest request,
        CancellationToken cancellationToken);
}