using Microsoft.EntityFrameworkCore;
using SweetShop.Application.Features.Addresses;
using SweetShop.Domain.Entities;
using SweetShop.Infrastructure.Persistence.Context;

namespace SweetShop.Infrastructure.Persistence.Stores;

/// <summary>
/// Represents a store for managing address entities in the database.
/// </summary>
public sealed class AddressStore : IAddressStore
{
    private readonly SweetShopDbContext dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddressStore"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public AddressStore(SweetShopDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        this.dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Address>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Customer ID is required.",
                nameof(customerId));
        }

        return await dbContext
            .Set<Address>()
            .Where(address => address.CustomerId == customerId)
            .OrderByDescending(address => address.IsDefault)
            .ThenBy(address => address.CreatedAt)
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Address?> GetByIdAsync(
        Guid addressId,
        CancellationToken cancellationToken)
    {
        if (addressId == Guid.Empty)
        {
            throw new ArgumentException(
                "Address ID is required.",
                nameof(addressId));
        }

        return await dbContext
            .Set<Address>()
            .SingleOrDefaultAsync(
                address => address.Id == addressId,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Address?> GetDefaultByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Customer ID is required.",
                nameof(customerId));
        }

        return await dbContext
            .Set<Address>()
            .SingleOrDefaultAsync(
                address =>
                    address.CustomerId == customerId &&
                    address.IsDefault,
                cancellationToken);
    }

    /// <inheritdoc />
    public void Add(Address address)
    {
        ArgumentNullException.ThrowIfNull(address);

        dbContext.Set<Address>().Add(address);
    }

    /// <inheritdoc />
    public void Remove(Address address)
    {
        ArgumentNullException.ThrowIfNull(address);

        dbContext.Set<Address>().Remove(address);
    }
}