using Microsoft.Extensions.Options;
using SweetShop.Application.Interfaces;

namespace SweetShop.Infrastructure.Configuration;

/// <summary>
/// Provides the current shop context from application configuration.
/// </summary>
public sealed class ShopContext : IShopContext
{
    private readonly ShopOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShopContext"/> class.
    /// </summary>
    /// <param name="options">The current shop configuration.</param>
    public ShopContext(IOptions<ShopOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        this.options = options.Value;

        if (this.options.Id == Guid.Empty)
        {
            throw new InvalidOperationException(
                "Shop ID is not configured.");
        }
    }

    /// <summary>
    /// Gets the identifier of the current shop.
    /// </summary>
    public Guid ShopId => options.Id;
}