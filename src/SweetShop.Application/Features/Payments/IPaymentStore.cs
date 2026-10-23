
using SweetShop.Domain.Entities;

namespace SweetShop.Application.Features.Payments;

/// <summary>
/// Defines persistence operations required for payment management.
/// </summary>
public interface IPaymentStore
{
    /// <summary>
    /// Gets an order by its identifier and owning customer.
    /// </summary>
    /// <param name="customerId">The customer who owns the order.</param>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The order when found; otherwise, <see langword="null"/>.</returns>
    Task<Order?> GetCustomerOrderAsync(
        Guid customerId,
        Guid orderId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets a payment by its identifier and owning customer's order.
    /// </summary>
    /// <param name="customerId">The customer who owns the payment's order.</param>
    /// <param name="paymentId">The payment identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The payment when found; otherwise, <see langword="null"/>.</returns>
    Task<Payment?> GetCustomerPaymentAsync(
        Guid customerId,
        Guid paymentId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets all payment attempts associated with an order.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The payments associated with the order.</returns>
    Task<IReadOnlyCollection<Payment>> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adds a payment to the current unit of work.
    /// </summary>
    /// <param name="payment">The payment to add.</param>
    void Add(Payment payment);
}
