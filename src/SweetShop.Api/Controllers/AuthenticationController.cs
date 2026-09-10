using Microsoft.AspNetCore.Mvc;
using SweetShop.Api.Common.Models;
using SweetShop.Application.Authentication;
using SweetShop.Application.Authentication.Requests;
using SweetShop.Application.Authentication.Responses;

namespace SweetShop.Api.Controllers;

/// <summary>
/// Represents the controller responsible for handling authentication-related operations, including OTP requests and verification.
/// </summary>
[ApiController]
[Route("api/v1/auth")]
public sealed class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService authenticationService;

/// <summary>
/// Initializes a new instance of the <see cref="AuthenticationController"/> class with the specified authentication service.
/// </summary>
/// <param name="authenticationService"></param>
/// <exception cref="ArgumentNullException"></exception>
    public AuthenticationController(
        IAuthenticationService authenticationService)
    {
        this.authenticationService = authenticationService
            ?? throw new ArgumentNullException(nameof(authenticationService));
    }
/// <summary>
/// Handles the request for a one-time password (OTP) to be sent to the user. If the request is valid, it returns the expiration time of the OTP.
/// </summary>
/// <param name="request"></param>
/// <param name="cancellationToken"></param>
/// <returns></returns>
    [HttpPost("request-otp")]
    [ProducesResponseType(typeof(ApiResponse<RequestOtpResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RequestOtp(
        [FromBody] RequestOtpRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authenticationService.RequestOtpAsync(
            request,
            cancellationToken);

        return Ok(new ApiResponse<RequestOtpResponse>(response));
    }

/// <summary>
/// Handles the verification of a one-time password (OTP) provided by the user. If the OTP is valid, it returns an access token and its expiration time.
/// </summary>
/// <param name="request"></param>
/// <param name="cancellationToken"></param>
/// <returns></returns>
    [HttpPost("verify-otp")]
    [ProducesResponseType(typeof(ApiResponse<VerifyOtpResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyOtp(
        [FromBody] VerifyOtpRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authenticationService.VerifyOtpAsync(
            request,
            cancellationToken);

        return Ok(new ApiResponse<VerifyOtpResponse>(response));
    }
}
