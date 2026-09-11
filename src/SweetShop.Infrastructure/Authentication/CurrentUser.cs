using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SweetShop.Application.Interfaces;

namespace SweetShop.Infrastructure.Authentication;

/// <summary>
/// Represents the currently authenticated user and provides access to their claims and authentication status.
/// </summary>
public sealed class CurrentUser : ICurrentUser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUser"/> class with the specified HTTP context accessor.
    /// </summary>
    private readonly IHttpContextAccessor httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUser"/> class with the specified HTTP context accessor.
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    public CurrentUser(
        IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor);

        this.httpContextAccessor = httpContextAccessor;
    }

/// <summary>
/// Gets the unique identifier of the currently authenticated user, if available.
/// </summary>
    public Guid? UserId
    {
        get
        {
            var subject = httpContextAccessor
                .HttpContext?
                .User
                .FindFirst("sub")?
                .Value;

            return Guid.TryParse(subject, out var userId)
                ? userId
                : null;
        }
    }

    /// <summary>
    /// Gets the role of the currently authenticated user, if available.
    /// </summary>
    public string? Role =>
        httpContextAccessor
            .HttpContext?
            .User
            .FindFirst(ClaimTypes.Role)?
            .Value;

    /// <summary>
    /// Gets a value indicating whether the current user is authenticated. 
    /// </summary>
    public bool IsAuthenticated =>
        httpContextAccessor
            .HttpContext?
            .User
            .Identity?
            .IsAuthenticated == true;
}