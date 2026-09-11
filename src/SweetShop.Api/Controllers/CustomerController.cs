using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetShop.Api.Common.Models;
using SweetShop.Application.Features.Customers;
using SweetShop.Application.Features.Customers.Requests;
using SweetShop.Application.Features.Customers.Responses;

namespace SweetShop.Api.Controllers;

/// <summary>
/// Represents the API controller responsible for handling customer-related operations, such as retrieving, creating, and updating customer profiles.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/customers")]
public sealed class CustomerController : ControllerBase
{
    private readonly ICustomerService customerService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerController"/> class with the specified <see cref="ICustomerService"/> dependency.
    /// </summary>
    /// <param name="customerService"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public CustomerController(
        ICustomerService customerService)
    {
        this.customerService = customerService
            ?? throw new ArgumentNullException(nameof(customerService));
    }

    /// <summary>
    /// Retrieves the profile of the currently authenticated customer. If the profile is not found, a 404 Not Found response is returned. Otherwise, a 200 OK response with the customer profile data is returned.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("me")]
    [ProducesResponseType(
    typeof(ApiResponse<CustomerProfileResponse>),
    StatusCodes.Status200OK)]
    [ProducesResponseType(
    typeof(ApiErrorResponse),
    StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyProfile(
        CancellationToken cancellationToken)
    {
        var response = await customerService.GetMyProfileAsync(
            cancellationToken);

        if (response is null)
        {
            return NotFound(
                new ApiErrorResponse(
                    new ApiError(
                        "CUSTOMER_PROFILE_NOT_FOUND",
                        "Customer profile was not found.",
                        Array.Empty<ApiErrorDetail>())));
        }

        return Ok(new ApiResponse<CustomerProfileResponse>(response));
    }

    /// <summary>
    /// Creates a new customer profile for the currently authenticated user. If the request data is valid, a 201 Created response with the created customer profile data is returned. If the request data is invalid, a 400 Bad Request response with validation error details is returned.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("me")]
    [ProducesResponseType(
        typeof(ApiResponse<CustomerProfileResponse>),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ApiErrorResponse),
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateMyProfile(
        [FromBody] CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var response = await customerService.CreateMyProfileAsync(
            request,
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            new ApiResponse<CustomerProfileResponse>(response));
    }

    /// <summary>
    /// Updates the profile of the currently authenticated customer. If the request data is valid, a 200 OK response with the updated customer profile data is returned. If the request data is invalid, a 400 Bad Request response with validation error details is returned.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("me")]
    [ProducesResponseType(
    typeof(ApiResponse<CustomerProfileResponse>),
    StatusCodes.Status200OK)]
    [ProducesResponseType(
    typeof(ApiErrorResponse),
    StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMyProfile(
        [FromBody] UpdateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var response = await customerService.UpdateMyProfileAsync(
            request,
            cancellationToken);

        return Ok(new ApiResponse<CustomerProfileResponse>(response));
    }
}