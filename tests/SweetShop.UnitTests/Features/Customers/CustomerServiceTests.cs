using SweetShop.Application.Authentication;
using SweetShop.Application.Features.Customers;
using SweetShop.Application.Features.Customers.Requests;
using SweetShop.Application.Interfaces;
using SweetShop.Domain.Entities;
using SweetShop.Domain.Enums;

namespace SweetShop.UnitTests.Features.Customers;

/// <summary>
/// Represents a suite of unit tests for the <see cref="CustomerService"/> class, verifying its behavior in various scenarios related to customer profile management.
/// </summary>
public sealed class CustomerServiceTests
{
    /// <summary>
    /// Tests the behavior of the <see cref="CustomerService.GetMyProfileAsync"/> method when the user is unauthenticated. It verifies that an <see cref="InvalidOperationException"/> is thrown with the expected message.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetMyProfileWhenUserIsUnauthenticatedThrows()
    {
        var currentUser = new FakeCurrentUser
        {
            IsAuthenticatedValue = false,
            UserIdValue = null
        };

        var service = CreateService(
            currentUser: currentUser);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.GetMyProfileAsync(CancellationToken.None));

        Assert.Equal(
            "Authenticated user is required.",
            exception.Message);
    }

    /// <summary>
    /// Tests the behavior of the <see cref="CustomerService.GetMyProfileAsync"/> method when the customer profile does not exist for the authenticated user. It verifies that the method returns null in this scenario.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetMyProfileWhenCustomerDoesNotExistReturnsNull()
    {
        var user = CreateUser();
        var currentUser = CreateAuthenticatedCurrentUser(user.Id);

        var userStore = new FakeUserStore(user);
        var customerStore = new FakeCustomerStore();

        var service = CreateService(
            currentUser,
            customerStore,
            userStore);

        var result = await service.GetMyProfileAsync(
            CancellationToken.None);

        Assert.Null(result);
    }

    /// <summary>
    /// Tests the behavior of the <see cref="CustomerService.CreateMyProfileAsync"/> method when a new customer profile is created for the authenticated user. It verifies that the created profile has the expected properties and that the customer store and unit of work are updated accordingly.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateMyProfileCreatesCustomerProfile()
    {
        var user = CreateUser();
        var currentUser = CreateAuthenticatedCurrentUser(user.Id);

        var userStore = new FakeUserStore(user);
        var customerStore = new FakeCustomerStore();
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            currentUser,
            customerStore,
            userStore,
            unitOfWork);

        var request = new CreateCustomerRequest(
            "Rahul Sharma",
            "rahul@example.com",
            "Preferred customer");

        var result = await service.CreateMyProfileAsync(
            request,
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(user.MobileNumber, result.MobileNumber);
        Assert.Equal("Rahul Sharma", result.Name);
        Assert.Equal("rahul@example.com", result.Email);
        Assert.Equal("Preferred customer", result.Notes);
        Assert.NotNull(customerStore.AddedCustomer);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    /// <summary>
    /// Tests the behavior of the <see cref="CustomerService.CreateMyProfileAsync"/> method when a customer profile already exists for the authenticated user. It verifies that an <see cref="InvalidOperationException"/> is thrown with the expected message.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateMyProfileWhenProfileAlreadyExistsThrows()
    {
        var user = CreateUser();
        var customer = new Customer(
            user.Id,
            "Existing Customer");

        var currentUser = CreateAuthenticatedCurrentUser(user.Id);

        var customerStore = new FakeCustomerStore(customer);
        var userStore = new FakeUserStore(user);

        var service = CreateService(
            currentUser,
            customerStore,
            userStore);

        var request = new CreateCustomerRequest(
            "New Customer",
            null,
            null);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateMyProfileAsync(
                request,
                CancellationToken.None));

        Assert.Equal(
            "Customer profile already exists.",
            exception.Message);
    }

    /// <summary>
    /// Tests the behavior of the <see cref="CustomerService.UpdateMyProfileAsync"/> method when updating an existing customer profile. It verifies that the updated profile has the expected properties and that the unit of work is updated accordingly.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpdateMyProfileUpdatesCustomerProfile()
    {
        var user = CreateUser();

        var customer = new Customer(
            user.Id,
            "Old Name");

        var currentUser = CreateAuthenticatedCurrentUser(user.Id);

        var customerStore = new FakeCustomerStore(customer);
        var userStore = new FakeUserStore(user);
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            currentUser,
            customerStore,
            userStore,
            unitOfWork);

        var request = new UpdateCustomerRequest(
            "Updated Name",
            "updated@example.com",
            "Updated notes");

        var result = await service.UpdateMyProfileAsync(
            request,
            CancellationToken.None);

        Assert.Equal(customer.Id, result.Id);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(user.MobileNumber, result.MobileNumber);
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal("updated@example.com", result.Email);
        Assert.Equal("Updated notes", result.Notes);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    /// <summary>
    /// Tests the behavior of the <see cref="CustomerService.UpdateMyProfileAsync"/> method when attempting to update a customer profile that does not exist for the authenticated user. It verifies that an <see cref="InvalidOperationException"/> is thrown with the expected message.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpdateMyProfileWhenProfileDoesNotExistThrows()
    {
        var user = CreateUser();
        var currentUser = CreateAuthenticatedCurrentUser(user.Id);

        var customerStore = new FakeCustomerStore();
        var userStore = new FakeUserStore(user);

        var service = CreateService(
            currentUser,
            customerStore,
            userStore);

        var request = new UpdateCustomerRequest(
            "Updated Name",
            null,
            null);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.UpdateMyProfileAsync(
                request,
                CancellationToken.None));

        Assert.Equal(
            "Customer profile was not found.",
            exception.Message);
    }

    /// <summary>
    /// Tests the behavior of the <see cref="CustomerService.GetMyProfileAsync"/> method to ensure that it correctly retrieves the customer profile using the mobile number of the authenticated user. It verifies that the returned profile has the expected mobile number.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetMyProfileUsesUserMobileNumber()
    {
        var user = CreateUser();

        var customer = new Customer(
            user.Id,
            "Customer Name");

        var currentUser = CreateAuthenticatedCurrentUser(user.Id);

        var customerStore = new FakeCustomerStore(customer);
        var userStore = new FakeUserStore(user);

        var service = CreateService(
            currentUser,
            customerStore,
            userStore);

        var result = await service.GetMyProfileAsync(
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(
            user.MobileNumber,
            result.MobileNumber);
    }

    private static CustomerService CreateService(
        FakeCurrentUser? currentUser = null,
        FakeCustomerStore? customerStore = null,
        FakeUserStore? userStore = null,
        FakeUnitOfWork? unitOfWork = null)
    {
        return new CustomerService(
            currentUser ?? CreateAuthenticatedCurrentUser(Guid.NewGuid()),
            customerStore ?? new FakeCustomerStore(),
            userStore ?? new FakeUserStore(),
            unitOfWork ?? new FakeUnitOfWork());
    }

    private static User CreateUser()
    {
        return new User(
            "9876543210",
            UserRole.Customer);
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

        public Customer? AddedCustomer { get; private set; }

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
            ArgumentNullException.ThrowIfNull(customer);

            AddedCustomer = customer;
        }
    }

    private sealed class FakeUserStore : IUserStore
    {
        private readonly User? user;

        public FakeUserStore(
            User? user = null)
        {
            this.user = user;
        }

        public Task<User?> GetByIdAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(
                user?.Id == userId
                    ? user
                    : null);
        }

        public Task<User?> GetByMobileNumberAsync(
            string mobileNumber,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(
                user?.MobileNumber == mobileNumber
                    ? user
                    : null);
        }

        public void Add(User user)
        {
            throw new NotSupportedException();
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