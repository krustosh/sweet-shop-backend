namespace SweetShop.Application.Authentication;

/// <summary>
/// Generates one-time passwords.
/// </summary>
public interface IOtpGenerator
{
    /// <summary>
    /// Generates a numeric one-time password.
    /// </summary>
    /// <returns>The generated OTP.</returns>
    string Generate();
}