using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SweetShop.Infrastructure.Authentication;
using Xunit;

namespace SweetShop.UnitTests.Authentication;

/// <summary>
/// Contains unit tests for the <see cref="JwtTokenService"/> class.
/// </summary>
public sealed class JwtTokenServiceTests
{
    private const string Issuer = "SweetShop.Api";
    private const string Audience = "SweetShop.Client";
    private const string SigningKey =
        "UNIT-TEST-ONLY-SIGNING-KEY-MUST-BE-AT-LEAST-32-CHARS";
    private const int ExpirationMinutes = 60;

    /// <summary>
    /// Tests that generating an access token with valid user data creates a valid JWT token with the expected claims and properties.
    /// </summary>
    [Fact]
    public void GenerateAccessTokenWithValidUserDataCreatesValidToken()
    {
        var service = CreateService();
        var userId = Guid.NewGuid();

        var token = service.GenerateAccessToken(
            userId,
            "9876543210",
            "Customer");

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        Assert.Equal(Issuer, jwt.Issuer);
        Assert.Contains(Audience, jwt.Audiences);
        Assert.Equal(
            userId.ToString(),
            jwt.Claims.First(
                claim => claim.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal(
            "9876543210",
            jwt.Claims.First(
                claim => claim.Type == JwtRegisteredClaimNames.UniqueName).Value);
        Assert.Equal(
            "Customer",
            jwt.Claims.First(
                claim => claim.Type == ClaimTypes.Role).Value);
    }

    /// <summary>
    /// Tests that generating an access token with an empty user ID throws an <see cref="ArgumentException"/>.
    /// </summary>
    [Fact]
    public void GenerateAccessTokenWithEmptyUserIdThrowsArgumentException()
    {
        var service = CreateService();

        Assert.Throws<ArgumentException>(() =>
            service.GenerateAccessToken(
                Guid.Empty,
                "9876543210",
                "Customer"));
    }
        
    /// <summary>
    /// Tests that generating an access token with an empty mobile number throws an <see cref="ArgumentException"/>.
    /// </summary>
    [Fact]
    public void GenerateAccessTokenWithEmptyMobileNumberThrowsArgumentException()
    {
        var service = CreateService();

        Assert.Throws<ArgumentException>(() =>
            service.GenerateAccessToken(
                Guid.NewGuid(),
                string.Empty,
                "Customer"));
    }

    /// <summary>
    /// Tests that generating an access token with an empty role throws an <see cref="ArgumentException"/>.
    /// </summary>
    [Fact]
    public void GenerateAccessTokenWithEmptyRoleThrowsArgumentException()
    {
        var service = CreateService();

        Assert.Throws<ArgumentException>(() =>
            service.GenerateAccessToken(
                Guid.NewGuid(),
                "9876543210",
                string.Empty));
    }

    /// <summary>
    /// Tests that the generated JWT token can be validated using the configured security parameters, ensuring that the token is correctly signed and contains the expected claims.
    /// </summary>
    [Fact]
    public void GeneratedTokenCanBeValidatedWithConfiguredSecurityParameters()
    {
        var service = CreateService();
        var userId = Guid.NewGuid();

        var token = service.GenerateAccessToken(
            userId,
            "9876543210",
            "Customer");

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Issuer,

            ValidateAudience = true,
            ValidAudience = Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(SigningKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        var handler = new JwtSecurityTokenHandler();

        var principal = handler.ValidateToken(
            token,
            validationParameters,
            out _);

        Assert.Equal(
            userId.ToString(),
            principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        Assert.Equal(
            "Customer",
            principal.FindFirst(ClaimTypes.Role)?.Value);
    }

    private static JwtTokenService CreateService()
    {
        var options = new JwtOptions
        {
            Issuer = Issuer,
            Audience = Audience,
            SigningKey = SigningKey,
            ExpirationMinutes = ExpirationMinutes
        };

        return new JwtTokenService(options);
    }
}