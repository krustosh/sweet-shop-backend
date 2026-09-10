using System.Security.Cryptography;
using System.Text;

using SweetShop.Application.Authentication;

namespace SweetShop.Infrastructure.Authentication;

/// <summary>
/// Provides SHA-256 hashing and verification for one-time passwords.
/// </summary>
public sealed class OtpHasher : IOtpHasher
{
    /// <inheritdoc />
    public string Hash(string otp)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(otp);

        var hash = SHA256.HashData(
            Encoding.UTF8.GetBytes(otp));

        return Convert.ToHexString(hash);
    }

    /// <inheritdoc />
    public bool Verify(
        string otp,
        string hash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(otp);
        ArgumentException.ThrowIfNullOrWhiteSpace(hash);

        var expectedHash = Hash(otp);

        return CryptographicOperations.FixedTimeEquals(
            Convert.FromHexString(expectedHash),
            Convert.FromHexString(hash));
    }
}