using Microsoft.Extensions.DependencyInjection;
using SweetShop.Application.Authentication;
using SweetShop.Application.Features.Customers;

namespace SweetShop.Application.DependencyInjection;

/// <summary>
/// Provides dependency injection registration for the Application layer.
/// </summary>
public static class ApplicationServiceCollectionExtensions
{
    /// <summary>
    /// Registers Application-layer services.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ICustomerService, CustomerService>();

        return services;
    }
}