using SweetShop.Domain.Entities;

namespace SweetShop.Application.Features.Addresses;

/// <summary>
/// Defines the contract for an address store that provides methods for managing addresses in the data store.
/// </summary>
public interface IAddressStore
{
    /// <summary>
    /// Gets a collection of addresses associated with a specific customer ID.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<Address>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets an address by its ID.
    /// </summary>
    /// <param name="addressId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Address?> GetByIdAsync(
        Guid addressId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets the default address associated with a specific customer ID.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Address?> GetDefaultByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new address to the data store.
    /// </summary>
    /// <param name="address"></param>
    void Add(Address address);

    /// <summary>
    /// Removes an address from the data store.
    /// </summary>
    /// <param name="address"></param>
    void Remove(Address address);
}