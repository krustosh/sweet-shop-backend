using SweetShop.Domain.Entities;

namespace SweetShop.Application.Features.Customers;

/// <summary>
/// Defines an interface for a customer store that provides methods for retrieving and adding customers.
/// </summary>
public interface ICustomerStore
{

    /// <summary>
    /// Retrieves a customer by their associated user ID.
    /// </summary>
    /// <param name="userId">The ID of the user associated with the customer.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The customer if found; otherwise, null.</returns>
    Task<Customer?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new customer to the store.
    /// </summary>
    /// <param name="customer">The customer to add.</param>
    void Add(Customer customer);
}