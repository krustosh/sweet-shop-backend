using Microsoft.EntityFrameworkCore;
using SweetShop.Domain.Entities;
using SweetShop.Domain.Enums;
using SweetShop.Domain.ValueObjects;
using SweetShop.Infrastructure.Persistence.Context;

namespace SweetShop.IntegrationTests.Infrastructure;

/// <summary>
/// Seeds deterministic catalog data required by integration tests.
/// </summary>
internal static class TestDatabaseSeeder
{
    private static readonly object SeedLock = new();

    /// <summary>
    /// Ensures the deterministic catalog hierarchy exists.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="shopId">The test shop identifier.</param>
    /// <param name="categoryId">The test category identifier.</param>
    /// <param name="productId">The test product identifier.</param>
    /// <param name="variantId">The test product variant identifier.</param>
    public static void SeedCatalog(
        SweetShopDbContext dbContext,
        Guid shopId,
        Guid categoryId,
        Guid productId,
        Guid variantId)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        lock (SeedLock)
        {
            SeedCatalogInternal(
                dbContext,
                shopId,
                categoryId,
                productId,
                variantId);
        }
    }

    /// <summary>
    /// Creates the deterministic catalog records when they do not already exist.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="shopId">The test shop identifier.</param>
    /// <param name="categoryId">The test category identifier.</param>
    /// <param name="productId">The test product identifier.</param>
    /// <param name="variantId">The test product variant identifier.</param>
    private static void SeedCatalogInternal(
        SweetShopDbContext dbContext,
        Guid shopId,
        Guid categoryId,
        Guid productId,
        Guid variantId)
    {
        var shop = dbContext.Set<Shop>()
            .SingleOrDefault(entity => entity.Id == shopId);

        if (shop is null)
        {
            shop = new Shop(
                "Integration Test Sweet Shop",
                "9999999999",
                "Integration Test Address");

            dbContext.Entry(shop)
                .Property(entity => entity.Id)
                .CurrentValue = shopId;

            dbContext.Set<Shop>().Add(shop);
        }

        var category = dbContext.Set<Category>()
            .SingleOrDefault(entity => entity.Id == categoryId);

        if (category is null)
        {
            category = new Category(
                shopId,
                "Integration Test Sweets",
                "Category used by integration tests.",
                null,
                1);

            dbContext.Entry(category)
                .Property(entity => entity.Id)
                .CurrentValue = categoryId;

            dbContext.Set<Category>().Add(category);
        }

        var product = dbContext.Set<Product>()
            .SingleOrDefault(entity => entity.Id == productId);

        if (product is null)
        {
            product = new Product(
                shopId,
                categoryId,
                "Integration Test Kaju Katli",
                "Product used by integration tests.",
                1);

            dbContext.Entry(product)
                .Property(entity => entity.Id)
                .CurrentValue = productId;

            dbContext.Set<Product>().Add(product);
        }

        var variant = dbContext.Set<ProductVariant>()
            .SingleOrDefault(entity => entity.Id == variantId);

        if (variant is null)
        {
            variant = new ProductVariant(
                productId,
                "500g",
                new ProductQuantity(
                    500,
                    ProductUnit.Kilogram),
                new Money(
                    450,
                    Domain.Common.DomainConstants.CurrencyInr),
                1);

            dbContext.Entry(variant)
                .Property(entity => entity.Id)
                .CurrentValue = variantId;

            dbContext.Set<ProductVariant>().Add(variant);
        }

        dbContext.SaveChanges();
    }
}
