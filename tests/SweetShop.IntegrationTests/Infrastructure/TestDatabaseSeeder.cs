using Microsoft.EntityFrameworkCore;
using SweetShop.Domain.Common;
using SweetShop.Domain.Entities;
using SweetShop.Domain.Enums;
using SweetShop.Domain.ValueObjects;
using SweetShop.Infrastructure.Persistence.Context;

namespace SweetShop.IntegrationTests.Infrastructure;

/// <summary>
/// Seeds deterministic data required by integration tests.
/// </summary>
internal static class TestDatabaseSeeder
{
    private static readonly object SeedLock = new();

    /// <summary>
    /// Seeds the deterministic shop catalog used by integration tests.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="shopId">The deterministic shop identifier.</param>
    /// <param name="categoryId">The deterministic category identifier.</param>
    /// <param name="productId">The deterministic product identifier.</param>
    /// <param name="variantId">The deterministic product variant identifier.</param>
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
    /// Seeds the deterministic customer used by integration tests.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userId">The preferred deterministic user identifier.</param>
    /// <param name="customerId">The deterministic customer identifier.</param>
    /// <param name="mobileNumber">The customer's mobile number.</param>
    /// <returns>The actual user identifier associated with the seeded customer.</returns>
    public static Guid SeedCustomer(
        SweetShopDbContext dbContext,
        Guid userId,
        Guid customerId,
        string mobileNumber)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        if (string.IsNullOrWhiteSpace(mobileNumber))
        {
            throw new ArgumentException(
                "Mobile number cannot be empty.",
                nameof(mobileNumber));
        }

        lock (SeedLock)
        {
            var user = dbContext.Set<User>()
                .SingleOrDefault(entity => entity.MobileNumber == mobileNumber);

            if (user is null)
            {
                user = new User(mobileNumber, UserRole.Customer);

                dbContext.Entry(user)
                    .Property(entity => entity.Id)
                    .CurrentValue = userId;

                dbContext.Set<User>().Add(user);
            }

            var customer = dbContext.Set<Customer>()
                .SingleOrDefault(entity => entity.UserId == user.Id);

            if (customer is null)
            {
                customer = new Customer(
                    user.Id,
                    "Integration Test Customer");

                dbContext.Entry(customer)
                    .Property(entity => entity.Id)
                    .CurrentValue = customerId;

                dbContext.Set<Customer>().Add(customer);
            }

            dbContext.SaveChanges();

            return user.Id;
        }
    }

    /// <summary>
    /// Seeds the catalog records while preserving deterministic identifiers.
    /// Existing test records are reactivated so previous integration test runs
    /// cannot leave the catalog unavailable to subsequent tests.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="shopId">The deterministic shop identifier.</param>
    /// <param name="categoryId">The deterministic category identifier.</param>
    /// <param name="productId">The deterministic product identifier.</param>
    /// <param name="variantId">The deterministic product variant identifier.</param>
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
        else
        {
            product.Activate();
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
                    DomainConstants.CurrencyInr),
                1);

            dbContext.Entry(variant)
                .Property(entity => entity.Id)
                .CurrentValue = variantId;

            dbContext.Set<ProductVariant>().Add(variant);
        }
        else
        {
            variant.Activate();
        }

        dbContext.SaveChanges();
    }
}