using SweetShop.Domain.Enums;

namespace SweetShop.Application.Features.Addresses.Requests;

/// <summary>
/// Represents the request model for updating an address.
/// </summary>
/// <param name="Label"></param>
/// <param name="RecipientName"></param>
/// <param name="MobileNumber"></param>
/// <param name="AddressLine1"></param>
/// <param name="AddressLine2"></param>
/// <param name="Landmark"></param>
/// <param name="Area"></param>
/// <param name="City"></param>
/// <param name="State"></param>
/// <param name="PostalCode"></param>
/// <param name="Latitude"></param>
/// <param name="Longitude"></param>
/// <param name="IsDefault"></param>
public sealed record UpdateAddressRequest(
    AddressLabel Label,
    string RecipientName,
    string MobileNumber,
    string AddressLine1,
    string? AddressLine2,
    string? Landmark,
    string Area,
    string City,
    string State,
    string PostalCode,
    decimal? Latitude,
    decimal? Longitude,
    bool IsDefault);