using SweetShop.Domain.Common;
using SweetShop.Domain.Enums;
namespace SweetShop.Domain.Entities;

/// <summary>
/// Represents a sweet shop in the platform.
/// </summary>
public sealed class Shop : AuditableEntity
{
    /// <summary>
    /// Gets the shop name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the shop description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the shop phone number.
    /// </summary>
    public string Phone { get; private set; }

    /// <summary>
    /// Gets the shop email address.
    /// </summary>
    public string? Email { get; private set; }

    /// <summary>
    /// Gets the shop address.
    /// </summary>
    public string Address { get; private set; }

    /// <summary>
    /// Gets the shop logo URL.
    /// </summary>
    public string? LogoUrl { get; private set; }

    /// <summary>
    /// Gets the current shop status.
    /// </summary>
    public ShopStatus Status { get; private set; }

    /// <summary>
    /// Initializes a new shop.
    /// </summary>
    /// <param name="name">The shop name.</param>
    /// <param name="phone">The shop phone number.</param>
    /// <param name="address">The shop address.</param>
    public Shop(
        string name,
        string phone,
        string address)
    {
        Name = RequireValue(name, nameof(name));
        Phone = RequireValue(phone, nameof(phone));
        Address = RequireValue(address, nameof(address));
        Status = ShopStatus.Active;
    }

    /// <summary>
    /// Updates the shop's basic information.
    /// </summary>
    /// <param name="name">The shop name.</param>
    /// <param name="phone">The shop phone number.</param>
    /// <param name="address">The shop address.</param>
    /// <param name="description">The shop description.</param>
    /// <param name="email">The shop email address.</param>
    /// <param name="logoUrl">The shop logo URL.</param>
    public void UpdateInformation(
        string name,
        string phone,
        string address,
        string? description,
        string? email,
        string? logoUrl)
    {
        Name = RequireValue(name, nameof(name));
        Phone = RequireValue(phone, nameof(phone));
        Address = RequireValue(address, nameof(address));
        Description = NormalizeOptional(description);
        Email = NormalizeOptional(email);
        LogoUrl = NormalizeOptional(logoUrl);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the shop.
    /// </summary>
    public void Activate()
    {
        Status = ShopStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the shop.
    /// </summary>
    public void Deactivate()
    {
        Status = ShopStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string RequireValue(string value, string parameterName)
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
