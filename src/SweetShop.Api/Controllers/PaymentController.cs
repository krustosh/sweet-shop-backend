using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetShop.Api.Common.Models;
using SweetShop.Application.Features.Payments;
using SweetShop.Application.Features.Payments.Requests;
using SweetShop.Application.Features.Payments.Responses;

namespace SweetShop.Api.Controllers;

/// <summary>
/// Provides customer-facing APIs for managing payments on authenticated customer orders.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/customers/me")]
public sealed class PaymentController : ControllerBase
{
    private readonly IPaymentService paymentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentController"/> class.
    /// </summary>
    /// <param name="paymentService">The payment application service.</param>
    public PaymentController(IPaymentService paymentService)
    {
        ArgumentNullException.ThrowIfNull(paymentService);

        this.paymentService = paymentService;
    }

    /// <summary>
    /// Initiates a payment for an order belonging to the authenticated customer.
    /// The payment amount is calculated from the saved order.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="request">The payment initiation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created payment record.</returns>
    [HttpPost("orders/{orderId:guid}/payments")]
    [ProducesResponseType(
        typeof(ApiResponse<PaymentResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> InitiatePayment(
        Guid orderId,
        [FromBody] InitiatePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var response = await paymentService.InitiateAsync(
            orderId,
            request,
            cancellationToken);

        return Ok(new ApiResponse<PaymentResponse>(response));
    }

    /// <summary>
    /// Gets a payment belonging to the authenticated customer.
    /// </summary>
    /// <param name="paymentId">The payment identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The requested payment, or a not-found response.</returns>
    [HttpGet("payments/{paymentId:guid}")]
    [ProducesResponseType(
        typeof(ApiResponse<PaymentResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyPayment(
        Guid paymentId,
        CancellationToken cancellationToken)
    {
        var response = await paymentService.GetMyPaymentAsync(
            paymentId,
            cancellationToken);

        if (response is null)
        {
            return NotFound(
                new ApiErrorResponse(
                    new ApiError(
                        "PAYMENT_NOT_FOUND",
                        "Payment was not found.",
                        Array.Empty<ApiErrorDetail>())));
        }

        return Ok(new ApiResponse<PaymentResponse>(response));
    }

    /// <summary>
    /// Gets all payment attempts for an order belonging to the authenticated customer.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The order's payment attempts, or a not-found response.</returns>
    [HttpGet("orders/{orderId:guid}/payments")]
    [ProducesResponseType(
        typeof(ApiResponse<IReadOnlyCollection<PaymentResponse>>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyOrderPayments(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var response = await paymentService.GetMyOrderPaymentsAsync(
            orderId,
            cancellationToken);

        if (response is null)
        {
            return NotFound(
                new ApiErrorResponse(
                    new ApiError(
                        "ORDER_NOT_FOUND",
                        "Order was not found.",
                        Array.Empty<ApiErrorDetail>())));
        }

        return Ok(
            new ApiResponse<IReadOnlyCollection<PaymentResponse>>(
                response));
    }
}
