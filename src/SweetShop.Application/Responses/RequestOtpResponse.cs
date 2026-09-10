namespace SweetShop.Application.Authentication.Responses;

/// <summary>
/// Represents the result of an OTP request.
/// </summary>
public sealed record RequestOtpResponse(
    DateTime ExpiresAt);