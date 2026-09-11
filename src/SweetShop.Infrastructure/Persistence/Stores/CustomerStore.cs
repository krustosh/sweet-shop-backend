using Microsoft.EntityFrameworkCore;
using SweetShop.Application.Features.Customers;
using SweetShop.Domain.Entities;
using SweetShop.Infrastructure.Persistence.Context;

namespace SweetShop.Infrastructure.Persistence.Stores;

/// <summary>
/// Represents a store for managing customer entities in the database.
/// </summary>
public sealed class CustomerStore : ICustomerStore
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerStore"/> class with the specified database context.
    /// </summary>
    private readonly SweetShopDbContext dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerStore"/> class with the specified database context.
    /// </summary>
    /// <param name="dbContext"></param>
    public CustomerStore(
        SweetShopDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        this.dbContext = dbContext;
    }

    /// <summary>
    /// Gets a customer entity by the specified user ID from the database, if available.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<Customer?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "User ID is required.",
                nameof(userId));
        }

        return await dbContext
            .Set<Customer>()
            .SingleOrDefaultAsync(
                customer => customer.UserId == userId,
                cancellationToken);
    }

    /// <summary>
    /// Adds a new customer entity to the database context.
    /// </summary>
    /// <param name="customer">The customer entity to add.</param>
    public void Add(Customer customer)
    {
        ArgumentNullException.ThrowIfNull(customer);

        dbContext.Set<Customer>().Add(customer);
    }
}