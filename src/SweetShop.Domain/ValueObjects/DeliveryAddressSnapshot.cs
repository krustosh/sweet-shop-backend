namespace SweetShop.Domain.ValueObjects;

/// <summary>
/// Represents the immutable delivery address captured when an order is placed.
/// </summary>
public sealed record DeliveryAddressSnapshot
{
    /// <summary>
    /// Gets the recipient's name.
    /// </summary>
    public string RecipientName { get; }

    /// <summary>
    /// Gets the recipient's mobile number.
    /// </summary>
    public string MobileNumber { get; }

    /// <summary>
    /// Gets the first address line.
    /// </summary>
    public string AddressLine1 { get; }

    /// <summary>
    /// Gets the second address line.
    /// </summary>
    public string? AddressLine2 { get; }

    /// <summary>
    /// Gets the landmark.
    /// </summary>
    public string? Landmark { get; }

    /// <summary>
    /// Gets the area or locality.
    /// </summary>
    public string Area { get; }

    /// <summary>
    /// Gets the city.
    /// </summary>
    public string City { get; }

    /// <summary>
    /// Gets the state.
    /// </summary>
    public string State { get; }

    /// <summary>
    /// Gets the postal code.
    /// </summary>
    public string PostalCode { get; }

    /// <summary>
    /// Gets the latitude coordinate.
    /// </summary>
    public decimal? Latitude { get; }

    /// <summary>
    /// Gets the longitude coordinate.
    /// </summary>
    public decimal? Longitude { get; }

    /// <summary>
    /// Initializes a delivery address snapshot.
    /// </summary>
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
    public DeliveryAddressSnapshot(
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
        RecipientName = RequireValue(
            recipientName,
            nameof(recipientName));

        MobileNumber = NormalizeMobileNumber(mobileNumber);

        AddressLine1 = RequireValue(
            addressLine1,
            nameof(addressLine1));

        AddressLine2 = NormalizeOptional(addressLine2);
        Landmark = NormalizeOptional(landmark);

        Area = RequireValue(area, nameof(area));
        City = RequireValue(city, nameof(city));
        State = RequireValue(state, nameof(state));
        PostalCode = RequireValue(postalCode, nameof(postalCode));

        Latitude = latitude;
        Longitude = longitude;
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