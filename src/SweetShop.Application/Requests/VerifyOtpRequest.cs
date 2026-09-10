namespace SweetShop.Application.Authentication.Requests;

/// <summary>
/// Represents a request to verify an OTP.
/// </summary>
public sealed record VerifyOtpRequest(
    string MobileNumber,
    string Otp);