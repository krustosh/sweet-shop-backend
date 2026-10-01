using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetShop.Api.Common.Models;
using SweetShop.Application.Features.Orders;
using SweetShop.Application.Features.Orders.Requests;
using SweetShop.Application.Features.Orders.Responses;

namespace SweetShop.Api.Controllers;

/// <summary>
/// Provides customer-facing APIs for managing authenticated customer orders.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/customers/me/orders")]
public sealed class OrderController : ControllerBase
{
    private readonly IOrderService orderService;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderController"/> class.
    /// </summary>
    /// <param name="orderService">The order application service.</param>
    public OrderController(IOrderService orderService)
    {
        ArgumentNullException.ThrowIfNull(orderService);

        this.orderService = orderService;
    }

    /// <summary>
    /// Gets all orders belonging to the authenticated customer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The customer's orders.</returns>
    [HttpGet]
    [ProducesResponseType(
        typeof(ApiResponse<IReadOnlyCollection<OrderResponse>>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyOrders(
        CancellationToken cancellationToken)
    {
        var response = await orderService.GetMyOrdersAsync(
            cancellationToken);

        return Ok(
            new ApiResponse<IReadOnlyCollection<OrderResponse>>(
                response));
    }

    /// <summary>
    /// Gets an order belonging to the authenticated customer.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The requested order.</returns>
    [HttpGet("{orderId:guid}")]
    [ProducesResponseType(
        typeof(ApiResponse<OrderResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyOrder(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var response = await orderService.GetMyOrderAsync(
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
            new ApiResponse<OrderResponse>(response));
    }

    /// <summary>
    /// Creates an order from the authenticated customer's active cart.
    /// </summary>
    /// <param name="request">The order creation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The newly created order.</returns>
    [HttpPost]
    [ProducesResponseType(
        typeof(ApiResponse<OrderResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var response = await orderService.CreateAsync(
            request,
            cancellationToken);

        return Ok(
            new ApiResponse<OrderResponse>(response));
    }

    /// <summary>
    /// Cancels an order belonging to the authenticated customer.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="request">The cancellation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The cancelled order.</returns>
    [HttpPost("{orderId:guid}/cancel")]
    [ProducesResponseType(
        typeof(ApiResponse<OrderResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(
        Guid orderId,
        [FromBody] CancelOrderRequest request,
        CancellationToken cancellationToken)
    {
        var response = await orderService.CancelAsync(
            orderId,
            request,
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
            new ApiResponse<OrderResponse>(response));
    }
}