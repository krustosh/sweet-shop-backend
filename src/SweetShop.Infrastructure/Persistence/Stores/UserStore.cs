using Microsoft.EntityFrameworkCore;
using SweetShop.Application.Authentication;
using SweetShop.Domain.Entities;
using SweetShop.Infrastructure.Persistence.Context;

namespace SweetShop.Infrastructure.Persistence.Stores;

/// <summary>
/// Provides Entity Framework Core persistence operations for authentication users.
/// </summary>
public sealed class UserStore : IUserStore
{
    private readonly SweetShopDbContext dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserStore"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public UserStore(SweetShopDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        this.dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<User?> GetByMobileNumberAsync(
        string mobileNumber,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mobileNumber);

        return await dbContext.Set<User>()
            .SingleOrDefaultAsync(
                user => user.MobileNumber == mobileNumber,
                cancellationToken);
    }

    /// <inheritdoc />
    public void Add(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        dbContext.Set<User>().Add(user);
    }
}