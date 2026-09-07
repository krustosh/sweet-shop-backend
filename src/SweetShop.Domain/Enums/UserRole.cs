namespace SweetShop.Domain.Enums;

/// <summary>
/// Defines the roles available to platform users.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Represents a customer user.
    /// </summary>
    Customer = 1,

    /// <summary>
    /// Represents an administrator.
    /// </summary>
    Admin = 2,

    /// <summary>
    /// Represents a delivery personnel user.
    /// </summary>
    Delivery = 3
}