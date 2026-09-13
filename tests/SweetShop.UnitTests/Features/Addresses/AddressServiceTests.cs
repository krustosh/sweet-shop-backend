using SweetShop.Application.Features.Addresses;
using SweetShop.Application.Features.Addresses.Requests;
using SweetShop.Application.Features.Customers;
using SweetShop.Application.Interfaces;
using SweetShop.Domain.Entities;
using SweetShop.Domain.Enums;

namespace SweetShop.UnitTests.Features.Addresses;

/// <summary>
/// Unit tests for the <see cref="AddressService"/> class.
/// </summary>
public sealed class AddressServiceTests
{
    /// <summary>
    /// Tests that the GetMyAddressesAsync method returns the addresses of the authenticated customer.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetMyAddressesReturnsCustomerAddresses()
    {
        var user = CreateUser();
        var customer = CreateCustomer(user);
        var address = CreateAddress(customer.Id);

        var service = CreateService(
            currentUser: CreateAuthenticatedCurrentUser(user.Id),
            customerStore: new FakeCustomerStore(customer),
            addressStore: new FakeAddressStore(address));

        var result = await service.GetMyAddressesAsync(
            CancellationToken.None);

        var returnedAddress = Assert.Single(result);

        Assert.Equal(address.Id, returnedAddress.Id);
        Assert.Equal(customer.Id, returnedAddress.CustomerId);
        Assert.Equal("John Doe", returnedAddress.RecipientName);
    }

    /// <summary>
    /// Tests that the GetMyAddressAsync method returns the address of the authenticated customer when the address belongs to them.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetMyAddressReturnsOwnedAddress()
    {
        var user = CreateUser();
        var customer = CreateCustomer(user);
        var address = CreateAddress(customer.Id);

        var service = CreateService(
            currentUser: CreateAuthenticatedCurrentUser(user.Id),
            customerStore: new FakeCustomerStore(customer),
            addressStore: new FakeAddressStore(address));

        var result = await service.GetMyAddressAsync(
            address.Id,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(address.Id, result.Id);
        Assert.Equal(customer.Id, result.CustomerId);
    }

    /// <summary>
    /// Tests that the GetMyAddressAsync method returns null when the address belongs to another customer.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetMyAddressWhenAddressBelongsToAnotherCustomerReturnsNull()
    {
        var user = CreateUser();
        var customer = CreateCustomer(user);

        var anotherUser = CreateUser("9876543211");
        var anotherCustomer = CreateCustomer(anotherUser);
        var address = CreateAddress(anotherCustomer.Id);

        var service = CreateService(
            currentUser: CreateAuthenticatedCurrentUser(user.Id),
            customerStore: new FakeCustomerStore(customer),
            addressStore: new FakeAddressStore(address));

        var result = await service.GetMyAddressAsync(
            address.Id,
            CancellationToken.None);

        Assert.Null(result);
    }

    /// <summary>
    /// Tests that the CreateMyAddressAsync method creates an address for the authenticated customer.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateMyAddressCreatesAddress()
    {
        var user = CreateUser();
        var customer = CreateCustomer(user);
        var addressStore = new FakeAddressStore();
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            currentUser: CreateAuthenticatedCurrentUser(user.Id),
            customerStore: new FakeCustomerStore(customer),
            addressStore: addressStore,
            unitOfWork: unitOfWork);

        var request = CreateAddressRequest();

        var result = await service.CreateMyAddressAsync(
            request,
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(customer.Id, result.CustomerId);
        Assert.Equal(AddressLabel.Home, result.Label);
        Assert.Equal("John Doe", result.RecipientName);
        Assert.Equal("9876543210", result.MobileNumber);
        Assert.Equal("123 Main Street", result.AddressLine1);
        Assert.Equal("Indiranagar", result.Area);
        Assert.Equal("Bengaluru", result.City);
        Assert.Equal("Karnataka", result.State);
        Assert.Equal("560038", result.PostalCode);
        Assert.Equal(12.971599m, result.Latitude);
        Assert.Equal(77.641157m, result.Longitude);
        Assert.False(result.IsDefault);
        Assert.NotNull(addressStore.AddedAddress);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    /// <summary>
    /// Tests that the CreateMyAddressAsync method removes the existing default address when a new default address is created.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateMyDefaultAddressRemovesExistingDefault()
    {
        var user = CreateUser();
        var customer = CreateCustomer(user);

        var existingDefault = CreateAddress(
            customer.Id,
            isDefault: true);

        var addressStore = new FakeAddressStore(existingDefault);
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            currentUser: CreateAuthenticatedCurrentUser(user.Id),
            customerStore: new FakeCustomerStore(customer),
            addressStore: addressStore,
            unitOfWork: unitOfWork);

        var request = CreateAddressRequest(isDefault: true);

        var result = await service.CreateMyAddressAsync(
            request,
            CancellationToken.None);

        Assert.True(result.IsDefault);
        Assert.False(existingDefault.IsDefault);
        Assert.NotNull(addressStore.AddedAddress);
        Assert.True(addressStore.AddedAddress.IsDefault);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    /// <summary>
    /// Tests that the UpdateMyAddressAsync method updates the address of the authenticated customer.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpdateMyAddressUpdatesAddress()
    {
        var user = CreateUser();
        var customer = CreateCustomer(user);
        var address = CreateAddress(customer.Id);
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            currentUser: CreateAuthenticatedCurrentUser(user.Id),
            customerStore: new FakeCustomerStore(customer),
            addressStore: new FakeAddressStore(address),
            unitOfWork: unitOfWork);

        var request = new UpdateAddressRequest(
            AddressLabel.Work,
            "Jane Doe",
            "9876543211",
            "456 Business Street",
            "Floor 2",
            "Near Metro",
            "Koramangala",
            "Bengaluru",
            "Karnataka",
            "560034",
            12.935192m,
            77.624481m,
            false);

        var result = await service.UpdateMyAddressAsync(
            address.Id,
            request,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(address.Id, result.Id);
        Assert.Equal(AddressLabel.Work, result.Label);
        Assert.Equal("Jane Doe", result.RecipientName);
        Assert.Equal("9876543211", result.MobileNumber);
        Assert.Equal("456 Business Street", result.AddressLine1);
        Assert.Equal("Floor 2", result.AddressLine2);
        Assert.Equal("Near Metro", result.Landmark);
        Assert.Equal("Koramangala", result.Area);
        Assert.Equal("560034", result.PostalCode);
        Assert.Equal(12.935192m, result.Latitude);
        Assert.Equal(77.624481m, result.Longitude);
        Assert.False(result.IsDefault);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    /// <summary>
    /// Tests that the UpdateMyAddressAsync method removes the existing default address when a new default address is updated.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpdateMyAddressAsDefaultRemovesExistingDefault()
    {
        var user = CreateUser();
        var customer = CreateCustomer(user);

        var existingDefault = CreateAddress(
            customer.Id,
            isDefault: true);

        var addressToUpdate = CreateAddress(customer.Id);

        var addressStore = new FakeAddressStore(
            existingDefault,
            addressToUpdate);

        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            currentUser: CreateAuthenticatedCurrentUser(user.Id),
            customerStore: new FakeCustomerStore(customer),
            addressStore: addressStore,
            unitOfWork: unitOfWork);

        var request = CreateAddressRequest(isDefault: true);

        var result = await service.UpdateMyAddressAsync(
            addressToUpdate.Id,
            new UpdateAddressRequest(
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
                request.Longitude,
                true),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsDefault);
        Assert.False(existingDefault.IsDefault);
        Assert.True(addressToUpdate.IsDefault);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    /// <summary>
    /// Tests that the UpdateMyAddressAsync method returns null when the address belongs to another customer.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpdateMyAddressWhenAddressBelongsToAnotherCustomerReturnsNull()
    {
        var user = CreateUser();
        var customer = CreateCustomer(user);

        var anotherUser = CreateUser("9876543211");
        var anotherCustomer = CreateCustomer(anotherUser);
        var address = CreateAddress(anotherCustomer.Id);

        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            currentUser: CreateAuthenticatedCurrentUser(user.Id),
            customerStore: new FakeCustomerStore(customer),
            addressStore: new FakeAddressStore(address),
            unitOfWork: unitOfWork);

        var result = await service.UpdateMyAddressAsync(
            address.Id,
            new UpdateAddressRequest(
                AddressLabel.Home,
                "Updated Recipient",
                "9876543210",
                "Updated Address",
                "Updated Address 2",
                "Updated Landmark",
                "Updated Area",
                "Updated City",
                "Updated State",
                "560002",
                12.9716m,
                77.5946m,
                true),
            CancellationToken.None);

        Assert.Null(result);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    /// <summary>
    /// Tests that the DeleteMyAddressAsync method deletes the address of the authenticated customer.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteMyAddressDeletesOwnedAddress()
    {
        var user = CreateUser();
        var customer = CreateCustomer(user);
        var address = CreateAddress(customer.Id);
        var addressStore = new FakeAddressStore(address);
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            currentUser: CreateAuthenticatedCurrentUser(user.Id),
            customerStore: new FakeCustomerStore(customer),
            addressStore: addressStore,
            unitOfWork: unitOfWork);

        var result = await service.DeleteMyAddressAsync(
            address.Id,
            CancellationToken.None);

        Assert.True(result);
        Assert.Same(address, addressStore.RemovedAddress);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    /// <summary>
    /// Tests that the DeleteMyAddressAsync method returns false when the address belongs to another customer.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteMyAddressWhenAddressBelongsToAnotherCustomerReturnsFalse()
    {
        var user = CreateUser();
        var customer = CreateCustomer(user);

        var anotherUser = CreateUser("9876543211");
        var anotherCustomer = CreateCustomer(anotherUser);
        var address = CreateAddress(anotherCustomer.Id);

        var addressStore = new FakeAddressStore(address);
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            currentUser: CreateAuthenticatedCurrentUser(user.Id),
            customerStore: new FakeCustomerStore(customer),
            addressStore: addressStore,
            unitOfWork: unitOfWork);

        var result = await service.DeleteMyAddressAsync(
            address.Id,
            CancellationToken.None);

        Assert.False(result);
        Assert.Null(addressStore.RemovedAddress);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    /// <summary>
    /// Tests that the GetMyAddressesAsync method throws an UnauthorizedAccessException when the user is unauthenticated.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetMyAddressesWhenUserIsUnauthenticatedThrows()
    {
        var currentUser = new FakeCurrentUser
        {
            IsAuthenticatedValue = false,
            UserIdValue = null
        };

        var service = CreateService(
            currentUser: currentUser);

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.GetMyAddressesAsync(CancellationToken.None));

        Assert.Equal(
            "Authenticated user ID was not found.",
            exception.Message);
    }

    private static AddressService CreateService(
        FakeCurrentUser? currentUser = null,
        FakeCustomerStore? customerStore = null,
        FakeAddressStore? addressStore = null,
        FakeUnitOfWork? unitOfWork = null)
    {
        return new AddressService(
            currentUser ?? CreateAuthenticatedCurrentUser(Guid.NewGuid()),
            customerStore ?? new FakeCustomerStore(),
            addressStore ?? new FakeAddressStore(),
            unitOfWork ?? new FakeUnitOfWork());
    }

    private static User CreateUser(
        string mobileNumber = "9876543210")
    {
        return new User(
            mobileNumber,
            UserRole.Customer);
    }

    private static Customer CreateCustomer(User user)
    {
        return new Customer(
            user.Id,
            "John Doe");
    }

    private static Address CreateAddress(
        Guid customerId,
        bool isDefault = false)
    {
        var address = new Address(
            customerId,
            AddressLabel.Home,
            "John Doe",
            "9876543210",
            "123 Main Street",
            "Indiranagar",
            "Bengaluru",
            "Karnataka",
            "560038");

        address.Update(
            AddressLabel.Home,
            "John Doe",
            "9876543210",
            "123 Main Street",
            null,
            null,
            "Indiranagar",
            "Bengaluru",
            "Karnataka",
            "560038",
            12.971599m,
            77.641157m);

        if (isDefault)
        {
            address.MarkAsDefault();
        }

        return address;
    }

    private static CreateAddressRequest CreateAddressRequest(
        bool isDefault = false)
    {
        return new CreateAddressRequest(
            AddressLabel.Home,
            "John Doe",
            "9876543210",
            "123 Main Street",
            null,
            null,
            "Indiranagar",
            "Bengaluru",
            "Karnataka",
            "560038",
            12.971599m,
            77.641157m,
            isDefault);
    }

    private static FakeCurrentUser CreateAuthenticatedCurrentUser(
        Guid userId)
    {
        return new FakeCurrentUser
        {
            IsAuthenticatedValue = true,
            UserIdValue = userId,
            RoleValue = UserRole.Customer.ToString()
        };
    }

    private sealed class FakeCurrentUser : ICurrentUser
    {
        public Guid? UserIdValue { get; init; }

        public string? RoleValue { get; init; }

        public bool IsAuthenticatedValue { get; init; }

        public Guid? UserId => UserIdValue;

        public string? Role => RoleValue;

        public bool IsAuthenticated => IsAuthenticatedValue;
    }

    private sealed class FakeCustomerStore : ICustomerStore
    {
        private readonly Customer? customer;

        public FakeCustomerStore(
            Customer? customer = null)
        {
            this.customer = customer;
        }

        public Task<Customer?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(
                customer?.UserId == userId
                    ? customer
                    : null);
        }

        public void Add(Customer customer)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class FakeAddressStore : IAddressStore
    {
        private readonly List<Address> addresses;

        public FakeAddressStore(params Address[] addresses)
        {
            this.addresses = addresses.ToList();
        }

        public Address? AddedAddress { get; private set; }

        public Address? RemovedAddress { get; private set; }

        public Task<IReadOnlyCollection<Address>> GetByCustomerIdAsync(
            Guid customerId,
            CancellationToken cancellationToken)
        {
            IReadOnlyCollection<Address> result = addresses
                .Where(address => address.CustomerId == customerId)
                .OrderByDescending(address => address.IsDefault)
                .ThenBy(address => address.CreatedAt)
                .ToArray();

            return Task.FromResult(result);
        }

        public Task<Address?> GetByIdAsync(
            Guid addressId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(
                addresses.SingleOrDefault(
                    address => address.Id == addressId));
        }

        public Task<Address?> GetDefaultByCustomerIdAsync(
            Guid customerId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(
                addresses.SingleOrDefault(
                    address =>
                        address.CustomerId == customerId &&
                        address.IsDefault));
        }

        public void Add(Address address)
        {
            ArgumentNullException.ThrowIfNull(address);

            AddedAddress = address;
            addresses.Add(address);
        }

        public void Remove(Address address)
        {
            ArgumentNullException.ThrowIfNull(address);

            RemovedAddress = address;
            addresses.Remove(address);
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;

            return Task.FromResult(1);
        }
    }
}