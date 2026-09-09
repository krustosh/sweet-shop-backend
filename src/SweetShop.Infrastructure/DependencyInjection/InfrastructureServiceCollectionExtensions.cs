using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SweetShop.Infrastructure.Persistence.Context;

namespace SweetShop.Infrastructure.DependencyInjection;

/// <summary>
/// Provides dependency injection registration for the Infrastructure layer.
/// </summary>
public static class InfrastructureServiceCollectionExtensions
{
    /// <summary>
    /// Registers Infrastructure-layer services.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString =
            configuration.GetSection(DatabaseOptions.SectionName)
                .Get<DatabaseOptions>()?.ConnectionString;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Database connection string is not configured.");
        }

        services.AddDbContext<SweetShopDbContext>(
            options => options.UseSqlServer(connectionString));

        return services;
    }
}