using SweetShop.Domain.Entities;

namespace SweetShop.Application.Authentication;

/// <summary>
/// Provides persistence operations for authentication users.
/// </summary>
public interface IUserStore
{
    /// <summary>
    /// Gets a user by mobile number.
    /// </summary>
    /// <param name="mobileNumber">The user's mobile number.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching user, or null when no user exists.</returns>
    Task<User?> GetByMobileNumberAsync(
        string mobileNumber,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new user.
    /// </summary>
    /// <param name="user">The user to add.</param>
    void Add(User user);
}