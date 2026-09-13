using SweetShop.Application.Interfaces;
using SweetShop.Application.Features.Addresses.Requests;
using SweetShop.Application.Features.Addresses.Responses;
using SweetShop.Domain.Entities;
using SweetShop.Application.Features.Customers;

namespace SweetShop.Application.Features.Addresses;

/// <summary>
/// Provides services for managing customer addresses.
/// </summary>
public sealed class AddressService : IAddressService
{
    private readonly ICurrentUser currentUser;
    private readonly ICustomerStore customerStore;
    private readonly IAddressStore addressStore;
    private readonly IUnitOfWork unitOfWork;

/// <summary>
/// Initializes a new instance of the <see cref="AddressService"/> class.
/// </summary>
/// <param name="currentUser"></param>
/// <param name="customerStore"></param>
/// <param name="addressStore"></param>
/// <param name="unitOfWork"></param>
    public AddressService(
        ICurrentUser currentUser,
        ICustomerStore customerStore,
        IAddressStore addressStore,
        IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(currentUser);
        ArgumentNullException.ThrowIfNull(customerStore);
        ArgumentNullException.ThrowIfNull(addressStore);
        ArgumentNullException.ThrowIfNull(unitOfWork);

        this.currentUser = currentUser;
        this.customerStore = customerStore;
        this.addressStore = addressStore;
        this.unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Gets the addresses of the currently authenticated customer.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IReadOnlyCollection<AddressResponse>> GetMyAddressesAsync(
        CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);

        var addresses = await addressStore.GetByCustomerIdAsync(
            customer.Id,
            cancellationToken);

        return addresses
            .Select(MapToResponse)
            .ToArray();
    }

    /// <summary>
    /// Gets a specific address of the currently authenticated customer by its ID.
    /// </summary>
    /// <param name="addressId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AddressResponse?> GetMyAddressAsync(
        Guid addressId,
        CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);

        var address = await addressStore.GetByIdAsync(
            addressId,
            cancellationToken);

        if (address is null || address.CustomerId != customer.Id)
        {
            return null;
        }

        return MapToResponse(address);
    }

    /// <summary>
    /// Creates a new address for the currently authenticated customer.
    /// If the new address is marked as default, any existing default address will be updated accordingly
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AddressResponse> CreateMyAddressAsync(
        CreateAddressRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var customer = await GetCurrentCustomerAsync(cancellationToken);

        if (request.IsDefault)
        {
            await RemoveExistingDefaultAsync(
                customer.Id,
                cancellationToken);
        }

        var address = new Address(
            customer.Id,
            request.Label,
            request.RecipientName,
            request.MobileNumber,
            request.AddressLine1,
            request.Area,
            request.City,
            request.State,
            request.PostalCode);

        address.Update(
            request.Label,
            request.RecipientName,
            request.MobileNumber,
            request.AddressLine1,
            request.AddressLine2,
            request.Landmark,
            request.Area,
            request.City,
            request.State,
            request.PostalCode,
            request.Latitude,
            request.Longitude);

        if (request.IsDefault)
        {
            address.MarkAsDefault();
        }

        addressStore.Add(address);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(address);
    }

    /// <summary>
    /// Updates an existing address of the currently authenticated customer.
    /// If the updated address is marked as default, any existing default address will be updated accordingly
    /// </summary>
    /// <param name="addressId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AddressResponse?> UpdateMyAddressAsync(
        Guid addressId,
        UpdateAddressRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var customer = await GetCurrentCustomerAsync(cancellationToken);

        var address = await addressStore.GetByIdAsync(
            addressId,
            cancellationToken);

        if (address is null || address.CustomerId != customer.Id)
        {
            return null;
        }

        if (request.IsDefault)
        {
            await RemoveExistingDefaultAsync(
                customer.Id,
                addressId,
                cancellationToken);

            address.MarkAsDefault();
        }
        else
        {
            address.RemoveDefaultDesignation();
        }

        address.Update(
            request.Label,
            request.RecipientName,
            request.MobileNumber,
            request.AddressLine1,
            request.AddressLine2,
            request.Landmark,
            request.Area,
            request.City,
            request.State,
            request.PostalCode,
            request.Latitude,
            request.Longitude);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(address);
    }

    /// <summary>
    /// Deletes an existing address of the currently authenticated customer.
    /// </summary>
    /// <param name="addressId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> DeleteMyAddressAsync(
        Guid addressId,
        CancellationToken cancellationToken)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);

        var address = await addressStore.GetByIdAsync(
            addressId,
            cancellationToken);

        if (address is null || address.CustomerId != customer.Id)
        {
            return false;
        }

        addressStore.Remove(address);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task<Domain.Entities.Customer> GetCurrentCustomerAsync(
        CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "Authenticated user ID was not found.");
        }

        var customer = await customerStore.GetByUserIdAsync(
            userId.Value,
            cancellationToken);

        if (customer is null)
        {
            throw new InvalidOperationException(
                "Customer profile was not found.");
        }

        return customer;
    }

    private async Task RemoveExistingDefaultAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        var existingDefault = await addressStore.GetDefaultByCustomerIdAsync(
            customerId,
            cancellationToken);

        existingDefault?.RemoveDefaultDesignation();
    }

    private async Task RemoveExistingDefaultAsync(
        Guid customerId,
        Guid addressId,
        CancellationToken cancellationToken)
    {
        var existingDefault = await addressStore.GetDefaultByCustomerIdAsync(
            customerId,
            cancellationToken);

        if (existingDefault is not null &&
            existingDefault.Id != addressId)
        {
            existingDefault.RemoveDefaultDesignation();
        }
    }

    private static AddressResponse MapToResponse(Address address)
    {
        return new AddressResponse(
            address.Id,
            address.CustomerId,
            address.Label,
            address.RecipientName,
            address.MobileNumber,
            address.AddressLine1,
            address.AddressLine2,
            address.Landmark,
            address.Area,
            address.City,
            address.State,
            address.PostalCode,
            address.Latitude,
            address.Longitude,
            address.IsDefault,
            address.CreatedAt,
            address.UpdatedAt);
    }
}