using Microsoft.EntityFrameworkCore;

namespace SweetShop.Infrastructure.Persistence.Context;

/// <summary>
/// Represents the Entity Framework Core database context for the Sweet Shop platform.
/// </summary>
public sealed class SweetShopDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SweetShopDbContext"/> class.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public SweetShopDbContext(DbContextOptions<SweetShopDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Configures the database model.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(SweetShopDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}