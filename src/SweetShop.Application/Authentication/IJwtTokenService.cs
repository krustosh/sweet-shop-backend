namespace SweetShop.Application.Authentication;

/// <summary>
/// Provides JWT access token generation.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates an access token for the specified user.
    /// </summary>
    /// <param name="userId">The authenticated user identifier.</param>
    /// <param name="mobileNumber">The authenticated user's mobile number.</param>
    /// <param name="role">The authenticated user's role.</param>
    /// <returns>The generated JWT access token.</returns>
    string GenerateAccessToken(
        Guid userId,
        string mobileNumber,
        string role);
}