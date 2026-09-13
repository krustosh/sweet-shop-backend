using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetShop.Api.Common.Models;
using SweetShop.Application.Features.Addresses;
using SweetShop.Application.Features.Addresses.Requests;
using SweetShop.Application.Features.Addresses.Responses;

namespace SweetShop.Api.Controllers;

/// <summary>
/// Controller for managing customer addresses.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/customers/me/addresses")]
public sealed class AddressController : ControllerBase
{
    private readonly IAddressService addressService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddressController"/> class.
    /// </summary>
    /// <param name="addressService"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public AddressController(IAddressService addressService)
    {
        this.addressService = addressService
            ?? throw new ArgumentNullException(nameof(addressService));
    }

    /// <summary>
    /// Retrieves the list of addresses associated with the authenticated customer.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(
        typeof(ApiResponse<IReadOnlyCollection<AddressResponse>>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyAddresses(
        CancellationToken cancellationToken)
    {
        var response = await addressService.GetMyAddressesAsync(
            cancellationToken);

        return Ok(
            new ApiResponse<IReadOnlyCollection<AddressResponse>>(response));
    }

    /// <summary>
    /// Retrieves a specific address associated with the authenticated customer by its unique identifier.
    /// </summary>
    /// <param name="addressId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{addressId:guid}")]
    [ProducesResponseType(
        typeof(ApiResponse<AddressResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyAddress(
        Guid addressId,
        CancellationToken cancellationToken)
    {
        var response = await addressService.GetMyAddressAsync(
            addressId,
            cancellationToken);

        if (response is null)
        {
            return NotFound(
                new ApiErrorResponse(
                    new ApiError(
                        "ADDRESS_NOT_FOUND",
                        "Address was not found.",
                        Array.Empty<ApiErrorDetail>())));
        }

        return Ok(new ApiResponse<AddressResponse>(response));
    }

    /// <summary>
    /// Creates a new address associated with the authenticated customer.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(
        typeof(ApiResponse<AddressResponse>),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateMyAddress(
        [FromBody] CreateAddressRequest request,
        CancellationToken cancellationToken)
    {
        var response = await addressService.CreateMyAddressAsync(
            request,
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            new ApiResponse<AddressResponse>(response));
    }

    /// <summary>
    /// Updates an existing address associated with the authenticated customer.
    /// </summary>
    /// <param name="addressId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{addressId:guid}")]
    [ProducesResponseType(
        typeof(ApiResponse<AddressResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMyAddress(
        Guid addressId,
        [FromBody] UpdateAddressRequest request,
        CancellationToken cancellationToken)
    {
        var response = await addressService.UpdateMyAddressAsync(
            addressId,
            request,
            cancellationToken);

        if (response is null)
        {
            return NotFound(
                new ApiErrorResponse(
                    new ApiError(
                        "ADDRESS_NOT_FOUND",
                        "Address was not found.",
                        Array.Empty<ApiErrorDetail>())));
        }

        return Ok(new ApiResponse<AddressResponse>(response));
    }

    /// <summary>
    /// Deletes an existing address associated with the authenticated customer.
    /// </summary>
    /// <param name="addressId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{addressId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMyAddress(
        Guid addressId,
        CancellationToken cancellationToken)
    {
        var deleted = await addressService.DeleteMyAddressAsync(
            addressId,
            cancellationToken);

        if (!deleted)
        {
            return NotFound(
                new ApiErrorResponse(
                    new ApiError(
                        "ADDRESS_NOT_FOUND",
                        "Address was not found.",
                        Array.Empty<ApiErrorDetail>())));
        }

        return NoContent();
    }
}