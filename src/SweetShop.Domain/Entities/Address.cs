using SweetShop.Domain.Common;
using SweetShop.Domain.Enums;

namespace SweetShop.Domain.Entities;

/// <summary>
/// Represents a saved delivery address belonging to a customer.
/// </summary>
public sealed class Address : AuditableEntity
{
    /// <summary>
    /// Gets the identifier of the customer who owns the address.
    /// </summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// Gets the address label.
    /// </summary>
    public AddressLabel Label { get; private set; }

    /// <summary>
    /// Gets the recipient's name.
    /// </summary>
    public string RecipientName { get; private set; }

    /// <summary>
    /// Gets the recipient's mobile number.
    /// </summary>
    public string MobileNumber { get; private set; }

    /// <summary>
    /// Gets the first address line.
    /// </summary>
    public string AddressLine1 { get; private set; }

    /// <summary>
    /// Gets the second address line.
    /// </summary>
    public string? AddressLine2 { get; private set; }

    /// <summary>
    /// Gets the landmark.
    /// </summary>
    public string? Landmark { get; private set; }

    /// <summary>
    /// Gets the area or locality.
    /// </summary>
    public string Area { get; private set; }

    /// <summary>
    /// Gets the city.
    /// </summary>
    public string City { get; private set; }

    /// <summary>
    /// Gets the state.
    /// </summary>
    public string State { get; private set; }

    /// <summary>
    /// Gets the postal code.
    /// </summary>
    public string PostalCode { get; private set; }

    /// <summary>
    /// Gets the optional latitude coordinate.
    /// </summary>
    public decimal? Latitude { get; private set; }

    /// <summary>
    /// Gets the optional longitude coordinate.
    /// </summary>
    public decimal? Longitude { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this is the customer's default address.
    /// </summary>
    public bool IsDefault { get; private set; }

    /// <summary>
    /// Initializes a new saved address.
    /// </summary>
    /// <param name="customerId">The identifier of the owning customer.</param>
    /// <param name="label">The address label.</param>
    /// <param name="recipientName">The recipient's name.</param>
    /// <param name="mobileNumber">The recipient's mobile number.</param>
    /// <param name="addressLine1">The first address line.</param>
    /// <param name="area">The area or locality.</param>
    /// <param name="city">The city.</param>
    /// <param name="state">The state.</param>
    /// <param name="postalCode">The postal code.</param>
    public Address(
        Guid customerId,
        AddressLabel label,
        string recipientName,
        string mobileNumber,
        string addressLine1,
        string area,
        string city,
        string state,
        string postalCode)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Customer ID cannot be empty.",
                nameof(customerId));
        }

        CustomerId = customerId;
        Label = label;
        RecipientName = RequireValue(recipientName, nameof(recipientName));
        MobileNumber = NormalizeMobileNumber(mobileNumber);
        AddressLine1 = RequireValue(addressLine1, nameof(addressLine1));
        Area = RequireValue(area, nameof(area));
        City = RequireValue(city, nameof(city));
        State = RequireValue(state, nameof(state));
        PostalCode = RequireValue(postalCode, nameof(postalCode));
    }

    /// <summary>
    /// Updates the saved address information.
    /// </summary>
    /// <param name="label">The address label.</param>
    /// <param name="recipientName">The recipient's name.</param>
    /// <param name="mobileNumber">The recipient's mobile number.</param>
    /// <param name="addressLine1">The first address line.</param>
    /// <param name="addressLine2">The second address line.</param>
    /// <param name="landmark">The landmark.</param>
    /// <param name="area">The area or locality.</param>
    /// <param name="city">The city.</param>
    /// <param name="state">The state.</param>
    /// <param name="postalCode">The postal code.</param>
    /// <param name="latitude">The latitude coordinate.</param>
    /// <param name="longitude">The longitude coordinate.</param>
    public void Update(
        AddressLabel label,
        string recipientName,
        string mobileNumber,
        string addressLine1,
        string? addressLine2,
        string? landmark,
        string area,
        string city,
        string state,
        string postalCode,
        decimal? latitude,
        decimal? longitude)
    {
        Label = label;
        RecipientName = RequireValue(recipientName, nameof(recipientName));
        MobileNumber = NormalizeMobileNumber(mobileNumber);
        AddressLine1 = RequireValue(addressLine1, nameof(addressLine1));
        AddressLine2 = NormalizeOptional(addressLine2);
        Landmark = NormalizeOptional(landmark);
        Area = RequireValue(area, nameof(area));
        City = RequireValue(city, nameof(city));
        State = RequireValue(state, nameof(state));
        PostalCode = RequireValue(postalCode, nameof(postalCode));
        Latitude = latitude;
        Longitude = longitude;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the address as the customer's default address.
    /// </summary>
    public void MarkAsDefault()
    {
        IsDefault = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Removes the default designation from the address.
    /// </summary>
    public void RemoveDefaultDesignation()
    {
        IsDefault = false;
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

    private static string NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Trim();
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