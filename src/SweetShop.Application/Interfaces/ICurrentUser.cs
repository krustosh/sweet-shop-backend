namespace SweetShop.Application.Interfaces;

/// <summary>
/// Provides information about the currently authenticated application user.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Gets the identifier of the current user.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Gets the role of the current user.
    /// </summary>
    string? Role { get; }

    /// <summary>
    /// Gets a value indicating whether a user is currently authenticated.
    /// </summary>
    bool IsAuthenticated { get; }
}