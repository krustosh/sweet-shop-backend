using SweetShop.Application.Features.Customers.Requests;
using SweetShop.Application.Features.Customers.Responses;

namespace SweetShop.Application.Features.Customers;

/// <summary>
/// Defines an interface for a customer service that provides methods for retrieving, creating, and updating customer profiles.
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// Retrieves the profile of the currently authenticated customer.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The customer profile if found; otherwise, null.</returns>
    Task<CustomerProfileResponse?> GetMyProfileAsync(
        CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new customer profile for the currently authenticated user.
    /// </summary>
    /// <param name="request">The request containing the customer details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created customer profile.</returns>
    Task<CustomerProfileResponse> CreateMyProfileAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Updates the profile of the currently authenticated customer.
    /// </summary>
    /// <param name="request">The request containing the updated customer details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated customer profile.</returns>
    Task<CustomerProfileResponse> UpdateMyProfileAsync(
        UpdateCustomerRequest request,
        CancellationToken cancellationToken);
}