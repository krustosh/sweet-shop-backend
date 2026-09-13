using SweetShop.Domain.Entities;

namespace SweetShop.Application.Features.Categories;

/// <summary>
/// Defines persistence operations for product categories.
/// </summary>
public interface ICategoryStore
{
    /// <summary>
    /// Gets all categories belonging to a shop.
    /// </summary>
    /// <param name="shopId">The shop identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The categories belonging to the shop.</returns>
    Task<IReadOnlyCollection<Category>> GetByShopIdAsync(
        Guid shopId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets all active categories belonging to a shop.
    /// </summary>
    /// <param name="shopId">The shop identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The active categories belonging to the shop.</returns>
    Task<IReadOnlyCollection<Category>> GetActiveByShopIdAsync(
        Guid shopId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets a category by its identifier.
    /// </summary>
    /// <param name="categoryId">The category identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="shopId">The shop identifier.</param>
    /// <returns>
    /// The category if found; otherwise, <see langword="null"/>.
    /// </returns>
    Task<Category?> GetByIdAsync(
    Guid shopId,
    Guid categoryId,
    CancellationToken cancellationToken);
    /// <summary>
    /// Gets a category by name within a shop.
    /// </summary>
    /// <param name="shopId">The shop identifier.</param>
    /// <param name="name">The category name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The matching category if found; otherwise, <see langword="null"/>.
    /// </returns>
    Task<Category?> GetByNameAsync(
        Guid shopId,
        string name,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adds a category to the current unit of work.
    /// </summary>
    /// <param name="category">The category to add.</param>
    void Add(Category category);
}