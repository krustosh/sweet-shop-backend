namespace SweetShop.Application.Authentication;

/// <summary>
/// Provides hashing and verification operations for one-time passwords.
/// </summary>
public interface IOtpHasher
{
    /// <summary>
    /// Hashes an OTP for secure persistence.
    /// </summary>
    /// <param name="otp">The plain-text OTP.</param>
    /// <returns>The hashed OTP.</returns>
    string Hash(string otp);

    /// <summary>
    /// Verifies an OTP against a previously generated hash.
    /// </summary>
    /// <param name="otp">The plain-text OTP.</param>
    /// <param name="hash">The previously generated hash.</param>
    /// <returns><c>true</c> when the OTP matches; otherwise <c>false</c>.</returns>
    bool Verify(
        string otp,
        string hash);
}