using SweetShop.Application.Authentication.Requests;
using SweetShop.Application.Authentication.Responses;

namespace SweetShop.Application.Authentication;

/// <summary>
/// Provides application-level authentication operations.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Requests an OTP for the specified mobile number.
    /// </summary>
    /// <param name="request">The OTP request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The OTP request result.</returns>
    Task<RequestOtpResponse> RequestOtpAsync(
        RequestOtpRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Verifies an OTP and generates an access token.
    /// </summary>
    /// <param name="request">The OTP verification request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The authentication result.</returns>
    Task<VerifyOtpResponse> VerifyOtpAsync(
        VerifyOtpRequest request,
        CancellationToken cancellationToken);
}