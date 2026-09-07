namespace SweetShop.Domain.Enums;

/// <summary>
/// Defines the lifecycle status of a user account.
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// The user account is active.
    /// </summary>
    Active = 1,

    /// <summary>
    /// The user account is inactive.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// The user account is blocked.
    /// </summary>
    Blocked = 3
}