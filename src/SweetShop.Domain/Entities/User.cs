using SweetShop.Domain.Common;
using SweetShop.Domain.Enums;

namespace SweetShop.Domain.Entities;

/// <summary>
/// Represents a platform user and authentication identity.
/// </summary>
public sealed class User : AuditableEntity
{
    /// <summary>
    /// Gets the user's Indian mobile number used for authentication.
    /// </summary>
    public string MobileNumber { get; private set; }

    /// <summary>
    /// Gets the user's platform role.
    /// </summary>
    public UserRole Role { get; private set; }

    /// <summary>
    /// Gets the user's account status.
    /// </summary>
    public UserStatus Status { get; private set; }

    /// <summary>
    /// Gets the date and time when the user last logged in.
    /// </summary>
    public DateTime? LastLoginAt { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the user is allowed to authenticate.
    /// </summary>
    public bool CanAuthenticate => Status == UserStatus.Active;

    /// <summary>
    /// Initializes a new user.
    /// </summary>
    /// <param name="mobileNumber">
    /// The Indian mobile number used for authentication.
    /// </param>
    /// <param name="role">The user's platform role.</param>
    public User(
        string mobileNumber,
        UserRole role)
    {
        MobileNumber = NormalizeMobileNumber(mobileNumber);
        ValidateRole(role);

        Role = role;
        Status = UserStatus.Active;
    }

    /// <summary>
    /// Records a successful user login.
    /// </summary>
    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Changes the user's role.
    /// </summary>
    /// <param name="role">The new user role.</param>
    public void ChangeRole(UserRole role)
    {
        ValidateRole(role);

        Role = role;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the user account.
    /// </summary>
    public void Activate()
    {
        Status = UserStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the user account.
    /// </summary>
    public void Deactivate()
    {
        Status = UserStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Blocks the user account.
    /// </summary>
    public void Block()
    {
        Status = UserStatus.Blocked;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateRole(UserRole role)
    {
        if (!Enum.IsDefined(role))
        {
            throw new ArgumentOutOfRangeException(
                nameof(role),
                role,
                "User role is invalid.");
        }
    }

    private static string NormalizeMobileNumber(string mobileNumber)
    {
        if (string.IsNullOrWhiteSpace(mobileNumber))
        {
            throw new ArgumentException(
                "Mobile number is required.",
                nameof(mobileNumber));
        }

        var normalized = mobileNumber.Trim();

        if (normalized.Length != 10 ||
            normalized[0] is < '6' or > '9' ||
            normalized.Any(character => character is < '0' or > '9'))
        {
            throw new ArgumentException(
                "Mobile number must be a valid Indian 10-digit mobile number.",
                nameof(mobileNumber));
        }

        return normalized;
    }
}