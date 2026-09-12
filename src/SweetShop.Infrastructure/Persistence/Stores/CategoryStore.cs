using Microsoft.EntityFrameworkCore;
using SweetShop.Application.Features.Categories;
using SweetShop.Domain.Entities;
using SweetShop.Domain.Enums;
using SweetShop.Infrastructure.Persistence.Context;

namespace SweetShop.Infrastructure.Persistence.Stores;

/// <summary>
/// Provides persistence operations for product categories.
/// </summary>
public sealed class CategoryStore : ICategoryStore
{
    private readonly SweetShopDbContext dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoryStore"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public CategoryStore(SweetShopDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        this.dbContext = dbContext;
    }

    /// <summary>
    /// Gets all categories belonging to the specified shop.
    /// </summary>
    /// <param name="shopId">The shop identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The categories belonging to the specified shop, ordered by display
    /// order and then name.
    /// </returns>
    public async Task<IReadOnlyCollection<Category>> GetByShopIdAsync(
        Guid shopId,
        CancellationToken cancellationToken)
    {
        if (shopId == Guid.Empty)
        {
            throw new ArgumentException(
                "Shop ID is required.",
                nameof(shopId));
        }

        return await dbContext.Set<Category>()
            .Where(category => category.ShopId == shopId)
            .OrderBy(category => category.DisplayOrder)
            .ThenBy(category => category.Name)
            .ToArrayAsync(cancellationToken);
    }

    /// <summary>
    /// Gets all active categories belonging to the specified shop.
    /// </summary>
    /// <param name="shopId">The shop identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The active categories belonging to the specified shop, ordered by
    /// display order and then name.
    /// </returns>
    public async Task<IReadOnlyCollection<Category>> GetActiveByShopIdAsync(
        Guid shopId,
        CancellationToken cancellationToken)
    {
        if (shopId == Guid.Empty)
        {
            throw new ArgumentException(
                "Shop ID is required.",
                nameof(shopId));
        }

        return await dbContext.Set<Category>()
            .Where(category =>
                category.ShopId == shopId &&
                category.Status == CategoryStatus.Active)
            .OrderBy(category => category.DisplayOrder)
            .ThenBy(category => category.Name)
            .ToArrayAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a category by its identifier within the specified shop.
    /// </summary>
    /// <param name="shopId">The shop identifier.</param>
    /// <param name="categoryId">The category identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The category if it belongs to the specified shop and exists;
    /// otherwise, <see langword="null"/>.
    /// </returns>
    public async Task<Category?> GetByIdAsync(
        Guid shopId,
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        if (shopId == Guid.Empty)
        {
            throw new ArgumentException(
                "Shop ID is required.",
                nameof(shopId));
        }

        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException(
                "Category ID is required.",
                nameof(categoryId));
        }

        return await dbContext.Set<Category>()
            .SingleOrDefaultAsync(
                category =>
                    category.ShopId == shopId &&
                    category.Id == categoryId,
                cancellationToken);
    }

    /// <summary>
    /// Gets a category with the specified name within a shop.
    /// </summary>
    /// <param name="shopId">The shop identifier.</param>
    /// <param name="name">The category name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The matching category if found; otherwise,
    /// <see langword="null"/>.
    /// </returns>
    public async Task<Category?> GetByNameAsync(
        Guid shopId,
        string name,
        CancellationToken cancellationToken)
    {
        if (shopId == Guid.Empty)
        {
            throw new ArgumentException(
                "Shop ID is required.",
                nameof(shopId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Category name is required.",
                nameof(name));
        }

        var normalizedName = name.Trim();

        return await dbContext.Set<Category>()
            .SingleOrDefaultAsync(
                category =>
                    category.ShopId == shopId &&
                    category.Name == normalizedName,
                cancellationToken);
    }

    /// <summary>
    /// Adds a category to the current unit of work.
    /// </summary>
    /// <param name="category">The category to add.</param>
    public void Add(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);

        dbContext.Set<Category>().Add(category);
    }
}