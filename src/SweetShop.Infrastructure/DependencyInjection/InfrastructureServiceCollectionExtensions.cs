using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SweetShop.Application.Authentication;
using SweetShop.Application.Features.Customers;
using SweetShop.Application.Interfaces;
using SweetShop.Infrastructure.Authentication;
using SweetShop.Infrastructure.Persistence;
using SweetShop.Infrastructure.Persistence.Context;
using SweetShop.Infrastructure.Persistence.Stores;
using SweetShop.Infrastructure.Configuration;

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

        var jwtOptions = configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>()
            ?? throw new InvalidOperationException(
                "JWT configuration is not configured.");

        if (string.IsNullOrWhiteSpace(jwtOptions.Issuer))
        {
            throw new InvalidOperationException(
                "JWT issuer is required.");
        }

        if (string.IsNullOrWhiteSpace(jwtOptions.Audience))
        {
            throw new InvalidOperationException(
                "JWT audience is required.");
        }

        if (string.IsNullOrWhiteSpace(jwtOptions.SigningKey))
        {
            throw new InvalidOperationException(
                "JWT signing key is required.");
        }

        if (jwtOptions.SigningKey.Length < 32)
        {
            throw new InvalidOperationException(
                "JWT signing key must contain at least 32 characters.");
        }

        if (jwtOptions.ExpirationMinutes <= 0)
        {
            throw new InvalidOperationException(
                "JWT expiration must be greater than zero.");
        }

        var shopOptions = configuration
            .GetSection(ShopOptions.SectionName)
            .Get<ShopOptions>()
            ?? throw new InvalidOperationException(
                "Shop configuration is not configured.");

        if (shopOptions.Id == Guid.Empty)
        {
            throw new InvalidOperationException(
                "Shop ID is not configured.");
        }

       services.Configure<ShopOptions>(
       configuration.GetSection(ShopOptions.SectionName));

        services.AddScoped<IShopContext, ShopContext>();

        services.AddSingleton(jwtOptions);
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IOtpGenerator, OtpGenerator>();
        services.AddSingleton<IOtpHasher, OtpHasher>();
        services.AddScoped<IUserStore, UserStore>();
        services.AddScoped<IOtpVerificationStore, OtpVerificationStore>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOtpSender, DevelopmentOtpSender>();
        services.AddScoped<ICustomerStore, CustomerStore>();

        return services;
    }
}