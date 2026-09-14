using SweetShop.Application.Features.Addresses.Requests;
using SweetShop.Application.Features.Addresses.Responses;

namespace SweetShop.Application.Features.Addresses;

/// <summary>
/// Defines the contract for an address service that provides methods for managing addresses for customers.
/// </summary>
public interface IAddressService
{
    /// <summary>
    /// Gets a collection of addresses associated with the currently authenticated customer.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<AddressResponse>> GetMyAddressesAsync(
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets an address by its ID for the currently authenticated customer.
    /// </summary>
    /// <param name="addressId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AddressResponse?> GetMyAddressAsync(
        Guid addressId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new address for the currently authenticated customer.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AddressResponse> CreateMyAddressAsync(
        CreateAddressRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing address for the currently authenticated customer.
    /// </summary>
    /// <param name="addressId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AddressResponse?> UpdateMyAddressAsync(
        Guid addressId,
        UpdateAddressRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Marks an address as the default address for the currently authenticated customer.
    /// </summary>
    /// <param name="addressId">The identifier of the address to make default.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated address, or <see langword="null"/> when the address does not belong to the customer.</returns>
    Task<AddressResponse?> SetMyDefaultAddressAsync(
        Guid addressId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an address for the currently authenticated customer.
    /// </summary>
    /// <param name="addressId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> DeleteMyAddressAsync(
        Guid addressId,
        CancellationToken cancellationToken);
}