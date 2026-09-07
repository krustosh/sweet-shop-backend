using SweetShop.Domain.Common;

namespace SweetShop.Domain.Entities;

/// <summary>
/// Represents the customer profile associated with a platform user.
/// </summary>
public sealed class Customer : AuditableEntity
{
    /// <summary>
    /// Gets the identifier of the associated user.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the customer's name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the customer's email address.
    /// </summary>
    public string? Email { get; private set; }

    /// <summary>
    /// Gets internal notes about the customer.
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Initializes a new customer profile.
    /// </summary>
    /// <param name="userId">The identifier of the associated user.</param>
    /// <param name="name">The customer's name.</param>
    public Customer(
        Guid userId,
        string name)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(userId));
        }

        UserId = userId;
        Name = RequireValue(name, nameof(name));
    }

    /// <summary>
    /// Updates the customer's profile information.
    /// </summary>
    /// <param name="name">The customer's name.</param>
    /// <param name="email">The customer's email address.</param>
    /// <param name="notes">Internal notes about the customer.</param>
    public void UpdateProfile(
        string name,
        string? email,
        string? notes)
    {
        Name = RequireValue(name, nameof(name));
        Email = NormalizeOptional(email);
        Notes = NormalizeOptional(notes);
        UpdatedAt = DateTime.UtcNow;
    }

    private static string RequireValue(
        string value,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "Value is required.",
                parameterName);
        }

        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}