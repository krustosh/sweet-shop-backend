using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SweetShop.Infrastructure.Persistence.Context;

/// <summary>
/// Creates the database context for Entity Framework Core design-time operations.
/// </summary>
public sealed class SweetShopDbContextFactory
    : IDesignTimeDbContextFactory<SweetShopDbContext>
{
    /// <summary>
    /// Creates a configured <see cref="SweetShopDbContext"/> for design-time operations.
    /// </summary>
    /// <param name="args">Command-line arguments supplied by Entity Framework Core.</param>
    /// <returns>A configured database context.</returns>
    public SweetShopDbContext CreateDbContext(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        var optionsBuilder = new DbContextOptionsBuilder<SweetShopDbContext>();

        var connectionString =
            Environment.GetEnvironmentVariable("SWEETSHOP_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "SWEETSHOP_CONNECTION_STRING environment variable is required for EF Core design-time operations.");
        }

        optionsBuilder.UseSqlServer(connectionString);

        return new SweetShopDbContext(optionsBuilder.Options);
    }
}