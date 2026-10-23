
using SweetShop.Application.Features.Payments.Requests;
using SweetShop.Application.Features.Payments.Responses;

namespace SweetShop.Application.Features.Payments;

/// <summary>
/// Defines customer-facing payment management operations.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Initiates a payment for an order belonging to the authenticated customer.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="request">The payment initiation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created payment.</returns>
    Task<PaymentResponse> InitiateAsync(
        Guid orderId,
        InitiatePaymentRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets a payment belonging to the authenticated customer.
    /// </summary>
    /// <param name="paymentId">The payment identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The payment when found; otherwise, <see langword="null"/>.</returns>
    Task<PaymentResponse?> GetMyPaymentAsync(
        Guid paymentId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets payment attempts for an order belonging to the authenticated customer.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The order's payment attempts, or <see langword="null"/> if the order is not found.</returns>
    Task<IReadOnlyCollection<PaymentResponse>?> GetMyOrderPaymentsAsync(
        Guid orderId,
        CancellationToken cancellationToken);
}
