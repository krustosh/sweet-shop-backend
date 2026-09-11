using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SweetShop.Api.Common.ExceptionHandling;
using SweetShop.Api.Common.Models;
using SweetShop.Api.Filters;
using SweetShop.Application.Authentication.Validators;
using SweetShop.Application.DependencyInjection;
using SweetShop.Infrastructure.Authentication;
using SweetShop.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(
    "appsettings.Local.json",
    optional: true,
    reloadOnChange: false);

builder.Services.AddOpenApi();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationActionFilter>();
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var details = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .SelectMany(entry => entry.Value!.Errors.Select(error =>
                new ApiErrorDetail(
                    entry.Key,
                    string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? "The supplied value is invalid."
                        : error.ErrorMessage)))
            .ToArray();

        return new BadRequestObjectResult(
            new ApiErrorResponse(
                new ApiError(
                    "VALIDATION_ERROR",
                    "One or more validation errors occurred.",
                    details)));
    };
});

builder.Services.AddScoped<ValidationActionFilter>();

builder.Services.AddValidatorsFromAssemblyContaining<RequestOtpRequestValidator>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

var jwtOptions = builder.Configuration
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
        "JWT signing key is not configured.");
}

if (jwtOptions.SigningKey.Length < 32)
{
    throw new InvalidOperationException(
        "JWT signing key must contain at least 32 characters.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();