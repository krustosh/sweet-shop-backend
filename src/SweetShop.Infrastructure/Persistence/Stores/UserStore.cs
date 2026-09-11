using Microsoft.EntityFrameworkCore;
using SweetShop.Application.Authentication;
using SweetShop.Domain.Entities;
using SweetShop.Infrastructure.Persistence.Context;

namespace SweetShop.Infrastructure.Persistence.Stores;

/// <summary>
/// Represents a store for managing user entities in the database.
/// </summary>
public sealed class UserStore : IUserStore
{
    private readonly SweetShopDbContext dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserStore"/> class with the specified database context.
    /// </summary>
    public UserStore(
        SweetShopDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        this.dbContext = dbContext;
    }

    /// <summary>
    /// Gets a user entity by the specified user ID from the database, if available.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<User?> GetByIdAsync(
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
            .Set<User>()
            .SingleOrDefaultAsync(
                user => user.Id == userId,
                cancellationToken);
    }

    /// <summary>
    /// Gets a user entity by the specified mobile number from the database, if available.
    /// </summary>
    /// <param name="mobileNumber">The mobile number of the user to retrieve.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<User?> GetByMobileNumberAsync(
        string mobileNumber,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mobileNumber);

        return await dbContext
            .Set<User>()
            .SingleOrDefaultAsync(
                user => user.MobileNumber == mobileNumber,
                cancellationToken);
    }

/// <summary>
/// Adds a new user entity to the database context.
/// </summary>
/// <param name="user">The user entity to add.</param>
    public void Add(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        dbContext.Set<User>().Add(user);
    }
}