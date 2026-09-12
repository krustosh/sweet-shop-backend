using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SweetShop.Application.Features.Categories;

namespace SweetShop.IntegrationTests.Infrastructure;

public sealed class SweetShopApiFactory : WebApplicationFactory<Program>
{
    public const string TestAuthenticationScheme = "TestAuthentication";

    public static Guid TestShopId { get; } =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

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
                _ =>
                {
                });
            services.RemoveAll<ICategoryService>();
            services.AddSingleton<ICategoryService, TestCategoryService>();
        });
    }
}

internal sealed class TestAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Test-Role", out var role)
            || string.IsNullOrWhiteSpace(role))
        {
            return Task.FromResult(
                AuthenticateResult.NoResult());
        }

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                "11111111-1111-1111-1111-111111111112"),

            new Claim(
                ClaimTypes.Role,
                role.ToString())
        };

        var identity = new ClaimsIdentity(
            claims,
            SweetShopApiFactory.TestAuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(
            principal,
            SweetShopApiFactory.TestAuthenticationScheme);

        return Task.FromResult(
            AuthenticateResult.Success(ticket));
    }
}