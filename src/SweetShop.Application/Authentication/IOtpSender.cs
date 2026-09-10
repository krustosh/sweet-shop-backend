namespace SweetShop.Application.Authentication;

/// <summary>
/// Sends one-time passwords to users.
/// </summary>
public interface IOtpSender
{
    /// <summary>
    /// Sends an OTP to the specified mobile number.
    /// </summary>
    /// <param name="mobileNumber">The recipient mobile number.</param>
    /// <param name="otp">The one-time password.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendAsync(
        string mobileNumber,
        string otp);
}