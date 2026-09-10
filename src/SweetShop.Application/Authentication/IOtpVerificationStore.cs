using SweetShop.Domain.Entities;

namespace SweetShop.Application.Authentication;

/// <summary>
/// Provides persistence operations for OTP verifications.
/// </summary>
public interface IOtpVerificationStore
{
    /// <summary>
    /// Gets the latest pending OTP verification for a mobile number.
    /// </summary>
    /// <param name="mobileNumber">The mobile number.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The pending OTP verification, or null when none exists.</returns>
    Task<OtpVerification?> GetPendingByMobileNumberAsync(
        string mobileNumber,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new OTP verification.
    /// </summary>
    /// <param name="verification">The OTP verification to add.</param>
    void Add(OtpVerification verification);
}