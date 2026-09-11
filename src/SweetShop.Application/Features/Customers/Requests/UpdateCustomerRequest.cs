namespace SweetShop.Application.Features.Customers.Requests;

/// <summary>
/// Represents a request to update an existing customer with the specified name, email, and notes.
/// </summary>
/// <param name="Name">The updated name of the customer.</param>
/// <param name="Email">The updated email address of the customer.</param>
/// <param name="Notes">Any additional notes about the customer.</param>
public sealed record UpdateCustomerRequest(
    string Name,
    string? Email,
    string? Notes);