namespace SweetShop.Application.Features.Customers.Requests;

/// <summary>
/// Represents a request to create a new customer with the specified name, email, and notes.
/// </summary>
/// <param name="Name">The name of the customer to create.</param>
/// <param name="Email">The email address of the customer to create.</param>
/// <param name="Notes">Any additional notes about the customer to create.</param>
public sealed record CreateCustomerRequest(
    string Name,
    string? Email,
    string? Notes);