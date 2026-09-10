namespace SweetShop.Application.Authentication.Responses;

/// <summary>
/// Represents the result of successful OTP verification.
/// </summary>
public sealed record VerifyOtpResponse(
    string AccessToken,
    DateTime ExpiresAt);