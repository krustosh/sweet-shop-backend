namespace SweetShop.Application.Authentication.Requests;

/// <summary>
/// Represents a request to send an OTP.
/// </summary>
public sealed record RequestOtpRequest(
    string MobileNumber);