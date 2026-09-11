using SweetShop.Domain.Entities;

namespace SweetShop.Application.Authentication;

/// <summary>
/// Defines a contract for managing user entities in the application.
/// </summary>
public interface IUserStore
{
    /// <summary>
    /// Gets a user entity by the specified user ID from the data store, if available.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<User?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets a user entity by the specified mobile number from the data store, if available.
    /// </summary>
    /// <param name="mobileNumber">The mobile number of the user to retrieve.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<User?> GetByMobileNumberAsync(
        string mobileNumber,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new user entity to the data store.
    /// </summary>
    /// <param name="user">The user entity to add.</param>
    void Add(User user);
}