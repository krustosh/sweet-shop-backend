using System.Security.Cryptography;
using System.Globalization;

using SweetShop.Application.Authentication;

namespace SweetShop.Infrastructure.Authentication;

/// <summary>
/// Generates cryptographically secure numeric one-time passwords.
/// </summary>
public sealed class OtpGenerator : IOtpGenerator
{
    private const int OtpLength = 6;
    private const int MinimumValue = 100000;
    private const int MaximumValue = 1000000;

    /// <inheritdoc />
    public string Generate()
    {
        var value = RandomNumberGenerator.GetInt32(
            MinimumValue,
            MaximumValue);

        return value.ToString($"D{OtpLength}", CultureInfo.InvariantCulture);
    }
}