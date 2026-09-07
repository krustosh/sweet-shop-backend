namespace SweetShop.Application.Interfaces;

/// <summary>
/// Represents the application abstraction for committing a unit of work.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Persists the current unit of work asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}