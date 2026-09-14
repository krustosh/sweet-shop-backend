using Microsoft.EntityFrameworkCore;
using SweetShop.Application.Features.Products;
using SweetShop.Domain.Entities;
using SweetShop.Domain.Enums;
using SweetShop.Infrastructure.Persistence.Context;

namespace SweetShop.Infrastructure.Persistence.Stores;

/// <summary>
/// Provides persistence operations for products.
/// </summary>
public sealed class ProductStore : IProductStore
{
    private readonly SweetShopDbContext dbContext;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbContext"></param>
    public ProductStore(SweetShopDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        this.dbContext = dbContext;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="shopId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IReadOnlyCollection<Product>> GetByShopIdAsync(
        Guid shopId,
        CancellationToken cancellationToken)
    {
        ValidateShopId(shopId);

        return await dbContext.Set<Product>()
            .Where(product => product.ShopId == shopId)
            .OrderBy(product => product.DisplayOrder)
            .ThenBy(product => product.Name)
            .ToArrayAsync(cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="shopId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IReadOnlyCollection<Product>> GetActiveByShopIdAsync(
        Guid shopId,
        CancellationToken cancellationToken)
    {
        ValidateShopId(shopId);

        return await dbContext.Set<Product>()
            .Where(product =>
                product.ShopId == shopId &&
                product.Status == ProductStatus.Active)
            .OrderBy(product => product.DisplayOrder)
            .ThenBy(product => product.Name)
            .ToArrayAsync(cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="shopId"></param>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<Product?> GetByIdAsync(
        Guid shopId,
        Guid productId,
        CancellationToken cancellationToken)
    {
        ValidateShopId(shopId);

        if (productId == Guid.Empty)
        {
            throw new ArgumentException(
                "Product ID is required.",
                nameof(productId));
        }

        return await dbContext.Set<Product>()
            .SingleOrDefaultAsync(
                product =>
                    product.ShopId == shopId &&
                    product.Id == productId,
                cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="shopId"></param>
    /// <param name="categoryId"></param>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<Product?> GetByNameAsync(
        Guid shopId,
        Guid categoryId,
        string name,
        CancellationToken cancellationToken)
    {
        ValidateShopId(shopId);

        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException(
                "Category ID is required.",
                nameof(categoryId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Product name is required.",
                nameof(name));
        }

        var normalizedName = name.Trim();

        return await dbContext.Set<Product>()
            .SingleOrDefaultAsync(
                product =>
                    product.ShopId == shopId &&
                    product.CategoryId == categoryId &&
                    product.Name == normalizedName,
                cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="product"></param>
    public void Add(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        dbContext.Set<Product>().Add(product);
    }

    private static void ValidateShopId(Guid shopId)
    {
        if (shopId == Guid.Empty)
        {
            throw new ArgumentException(
                "Shop ID is required.",
                nameof(shopId));
        }
    }
}