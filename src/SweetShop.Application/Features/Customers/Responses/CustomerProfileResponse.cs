namespace SweetShop.Application.Features.Customers.Responses;

/// <summary>
/// Represents a response containing the profile information of a customer, including their ID, user ID, mobile number, name, email, notes, creation date, and last updated date.
/// </summary>
/// <param name="Id">The ID of the customer.</param>
/// <param name="UserId">The ID of the user associated with the customer.</param>
/// <param name="MobileNumber">The mobile number of the customer.</param>
/// <param name="Name">The name of the customer.</param>
/// <param name="Email">The email address of the customer.</param>
/// <param name="Notes">Any additional notes about the customer.</param>
/// <param name="CreatedAt">The date and time when the customer was created.</param>
/// <param name="UpdatedAt">The date and time when the customer was last updated.</param>
public sealed record CustomerProfileResponse(
    Guid Id,
    Guid UserId,
    string MobileNumber,
    string Name,
    string? Email,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt);