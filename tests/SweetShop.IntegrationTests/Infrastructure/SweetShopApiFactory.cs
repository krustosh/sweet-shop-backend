using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SweetShop.Application.Features.Categories;
using SweetShop.Application.Features.ProductImages;
using SweetShop.Application.Features.Products;
using SweetShop.Infrastructure.Persistence.Context;

namespace SweetShop.IntegrationTests.Infrastructure;

/// <summary>
/// Provides a configured application factory for integration tests.
/// </summary>
public sealed class SweetShopApiFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// Gets the deterministic shop identifier used by integration tests.
    /// </summary>
    public static Guid TestShopId { get; } =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    /// <summary>
    /// Gets the deterministic category identifier used by integration tests.
    /// </summary>
    public static Guid TestCategoryId { get; } =
        Guid.Parse("22222222-2222-2222-2222-222222222222");

    /// <summary>
    /// Gets the deterministic product identifier used by integration tests.
    /// </summary>
    public static Guid TestProductId { get; } =
        Guid.Parse("33333333-3333-3333-3333-333333333333");

    /// <summary>
    /// Gets the deterministic product variant identifier used by integration tests.
    /// </summary>
    public static Guid TestVariantId { get; } =
        Guid.Parse("55555555-5555-5555-5555-555555555555");

    /// <summary>
    /// Gets the authentication scheme used by integration tests.
    /// </summary>
    public const string TestAuthenticationScheme = "TestAuthentication";

    /// <summary>
    /// Configures the test web host.
    /// </summary>
    /// <param name="builder">The web host builder.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseSetting(
            "Shop:Id",
            TestShopId.ToString());

        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthenticationScheme;
                options.DefaultChallengeScheme = TestAuthenticationScheme;
                options.DefaultForbidScheme = TestAuthenticationScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                TestAuthenticationScheme,
                _ => { });

            services.RemoveAll<ICategoryService>();
            services.AddSingleton<ICategoryService, TestCategoryService>();

            services.RemoveAll<IProductService>();
            services.AddSingleton<IProductService, TestProductService>();

            services.RemoveAll<IProductImageService>();
            services.AddSingleton<IProductImageService, TestProductImageService>();
        });
    }

    /// <summary>
    /// Creates the application host and ensures required integration-test data exists.
    /// </summary>
    /// <param name="builder">The application host builder.</param>
    /// <returns>The configured application host.</returns>
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<SweetShopDbContext>();

        dbContext.Database.Migrate();

        TestDatabaseSeeder.SeedCatalog(
            dbContext,
            TestShopId,
            TestCategoryId,
            TestProductId,
            TestVariantId);

        return host;
    }
}

/// <summary>
/// Provides the authentication handler used by integration tests.
/// </summary>
internal sealed class TestAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestAuthenticationHandler"/> class.
    /// </summary>
    /// <param name="options">The authentication options.</param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="encoder">The URL encoder.</param>
    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    /// <summary>
    /// Authenticates the current test request.
    /// </summary>
    /// <returns>The authentication result.</returns>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Test-Role", out var role) ||
            string.IsNullOrWhiteSpace(role))
        {
            return Task.FromResult(
                AuthenticateResult.NoResult());
        }

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                Guid.NewGuid().ToString()),
            new Claim(
                ClaimTypes.Role,
                role.ToString())
        };

        var identity = new ClaimsIdentity(
            claims,
            Scheme.Name);

        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(
            principal,
            Scheme.Name);

        return Task.FromResult(
            AuthenticateResult.Success(ticket));
    }
}