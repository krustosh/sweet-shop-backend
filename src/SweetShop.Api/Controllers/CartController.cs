using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetShop.Api.Common.Models;
using SweetShop.Application.Features.Cart;
using SweetShop.Application.Features.Cart.Requests;
using SweetShop.Application.Features.Cart.Responses;

namespace SweetShop.Api.Controllers;

/// <summary>
/// Provides customer-facing APIs for managing the authenticated customer's shopping cart.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/customers/me/cart")]
public sealed class CartController : ControllerBase
{
    private readonly ICartService cartService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CartController"/> class.
    /// </summary>
    /// <param name="cartService">The cart application service.</param>
    public CartController(ICartService cartService)
    {
        ArgumentNullException.ThrowIfNull(cartService);

        this.cartService = cartService;
    }

    /// <summary>
    /// Gets the active cart of the authenticated customer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The customer's active cart.</returns>
    [HttpGet]
    [ProducesResponseType(
        typeof(ApiResponse<CartResponse>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyCart(
        CancellationToken cancellationToken)
    {
        var response = await cartService.GetMyCartAsync(
            cancellationToken);

        return Ok(new ApiResponse<CartResponse>(response));
    }

    /// <summary>
    /// Adds a product variant to the authenticated customer's cart.
    /// </summary>
    /// <param name="request">The add-item request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated cart.</returns>
    [HttpPost("items")]
    [ProducesResponseType(
        typeof(ApiResponse<CartResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddItem(
        [FromBody] AddCartItemRequest request,
        CancellationToken cancellationToken)
    {
        var response = await cartService.AddItemAsync(
            request,
            cancellationToken);

        return Ok(new ApiResponse<CartResponse>(response));
    }

    /// <summary>
    /// Updates the quantity of an item in the authenticated customer's cart.
    /// </summary>
    /// <param name="cartItemId">The cart item identifier.</param>
    /// <param name="request">The quantity update request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated cart.</returns>
    [HttpPut("items/{cartItemId:guid}")]
    [ProducesResponseType(
        typeof(ApiResponse<CartResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItem(
        Guid cartItemId,
        [FromBody] UpdateCartItemRequest request,
        CancellationToken cancellationToken)
    {
        var response = await cartService.UpdateItemAsync(
            cartItemId,
            request,
            cancellationToken);

        if (response is null)
        {
            return NotFound(new ApiErrorResponse(
                new ApiError(
                    "CART_ITEM_NOT_FOUND",
                    "Cart item was not found.",
                    Array.Empty<ApiErrorDetail>())));
        }

        return Ok(new ApiResponse<CartResponse>(response));
    }

    /// <summary>
    /// Removes an item from the authenticated customer's cart.
    /// </summary>
    /// <param name="cartItemId">The cart item identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content when the item was removed.</returns>
    [HttpDelete("items/{cartItemId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveItem(
        Guid cartItemId,
        CancellationToken cancellationToken)
    {
        var removed = await cartService.RemoveItemAsync(
            cartItemId,
            cancellationToken);

        if (!removed)
        {
            return NotFound(new ApiErrorResponse(
                new ApiError(
                    "CART_ITEM_NOT_FOUND",
                    "Cart item was not found.",
                    Array.Empty<ApiErrorDetail>())));
        }

        return NoContent();
    }

    /// <summary>
    /// Removes all items from the authenticated customer's cart.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content when the cart was cleared.</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Clear(
        CancellationToken cancellationToken)
    {
        await cartService.ClearAsync(cancellationToken);

        return NoContent();
    }
}
