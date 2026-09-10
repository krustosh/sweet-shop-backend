using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SweetShop.Application.Authentication;

namespace SweetShop.Infrastructure.Authentication;

/// <summary>
/// Provides JWT access token generation.
/// </summary>
public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtTokenService"/> class.
    /// </summary>
    /// <param name="options">JWT configuration options.</param>
    public JwtTokenService(JwtOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(options.Issuer))
        {
            throw new ArgumentException(
                "JWT issuer is required.",
                nameof(options));
        }

        if (string.IsNullOrWhiteSpace(options.Audience))
        {
            throw new ArgumentException(
                "JWT audience is required.",
                nameof(options));
        }

        if (string.IsNullOrWhiteSpace(options.SigningKey))
        {
            throw new ArgumentException(
                "JWT signing key is required.",
                nameof(options));
        }

        if (options.SigningKey.Length < 32)
        {
            throw new ArgumentException(
                "JWT signing key must contain at least 32 characters.",
                nameof(options));
        }

        if (options.ExpirationMinutes <= 0)
        {
            throw new ArgumentException(
                "JWT expiration must be greater than zero.",
                nameof(options));
        }

        this.options = options;
    }

    /// <inheritdoc />
    public string GenerateAccessToken(
        Guid userId,
        string mobileNumber,
        string role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mobileNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(role);

        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "User identifier is required.",
                nameof(userId));
        }

        var claims = new[]
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                userId.ToString()),
            new Claim(
                JwtRegisteredClaimNames.UniqueName,
                mobileNumber),
            new Claim(
                ClaimTypes.Role,
                role)
        };

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(options.SigningKey));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                options.ExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}