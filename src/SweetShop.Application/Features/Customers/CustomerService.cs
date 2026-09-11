using SweetShop.Application.Authentication;
using SweetShop.Application.Features.Customers.Requests;
using SweetShop.Application.Features.Customers.Responses;
using SweetShop.Application.Interfaces;
using SweetShop.Domain.Entities;

namespace SweetShop.Application.Features.Customers;

/// <summary>
/// Represents a service for managing customer profiles and related operations.
/// </summary>
public sealed class CustomerService : ICustomerService
{
    private readonly ICurrentUser currentUser;
    private readonly ICustomerStore customerStore;
    private readonly IUserStore userStore;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerService"/> class with the specified dependencies.
    /// </summary>
    /// <param name="currentUser"></param>
    /// <param name="customerStore"></param>
    /// <param name="userStore"></param>
    /// <param name="unitOfWork"></param>
    public CustomerService(
        ICurrentUser currentUser,
        ICustomerStore customerStore,
        IUserStore userStore,
        IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(currentUser);
        ArgumentNullException.ThrowIfNull(customerStore);
        ArgumentNullException.ThrowIfNull(userStore);
        ArgumentNullException.ThrowIfNull(unitOfWork);

        this.currentUser = currentUser;
        this.customerStore = customerStore;
        this.userStore = userStore;
        this.unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Gets the profile of the currently authenticated customer, if available.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<CustomerProfileResponse?> GetMyProfileAsync(
        CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();

        var customer = await customerStore.GetByUserIdAsync(
            userId,
            cancellationToken);

        if (customer is null)
        {
            return null;
        }

        var user = await userStore.GetByIdAsync(
            userId,
            cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException(
                "Authenticated user was not found.");
        }

        return MapToResponse(customer, user);
    }

    /// <summary>
    /// Creates a new customer profile for the currently authenticated user based on the provided request data.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<CustomerProfileResponse> CreateMyProfileAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userId = GetAuthenticatedUserId();

        var existingCustomer = await customerStore.GetByUserIdAsync(
            userId,
            cancellationToken);

        if (existingCustomer is not null)
        {
            throw new InvalidOperationException(
                "Customer profile already exists.");
        }

        var user = await userStore.GetByIdAsync(
            userId,
            cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException(
                "Authenticated user was not found.");
        }

        var customer = new Customer(
            userId,
            request.Name);

        customer.UpdateProfile(
            request.Name,
            request.Email,
            request.Notes);

        customerStore.Add(customer);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(customer, user);
    }

    /// <summary>
    /// Updates the profile of the currently authenticated customer based on the provided request data.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<CustomerProfileResponse> UpdateMyProfileAsync(
        UpdateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userId = GetAuthenticatedUserId();

        var customer = await customerStore.GetByUserIdAsync(
            userId,
            cancellationToken);

        if (customer is null)
        {
            throw new InvalidOperationException(
                "Customer profile was not found.");
        }

        var user = await userStore.GetByIdAsync(
            userId,
            cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException(
                "Authenticated user was not found.");
        }

        customer.UpdateProfile(
            request.Name,
            request.Email,
            request.Notes);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(customer, user);
    }

    private Guid GetAuthenticatedUserId()
    {
        if (!currentUser.IsAuthenticated ||
            currentUser.UserId is null ||
            currentUser.UserId == Guid.Empty)
        {
            throw new InvalidOperationException(
                "Authenticated user is required.");
        }

        return currentUser.UserId.Value;
    }

    private static CustomerProfileResponse MapToResponse(
        Customer customer,
        User user)
    {
        return new CustomerProfileResponse(
            customer.Id,
            customer.UserId,
            user.MobileNumber,
            customer.Name,
            customer.Email,
            customer.Notes,
            customer.CreatedAt,
            customer.UpdatedAt);
    }
}