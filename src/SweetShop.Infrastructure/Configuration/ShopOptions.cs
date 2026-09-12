namespace SweetShop.Infrastructure.Configuration;
/// <summary>
/// Represents configuration for the current shop used by the application.
/// </summary>
public sealed class ShopOptions
{
    /// <summary>
    /// Gets the configuration section name.
    /// </summary>
    public const string SectionName = "Shop";

    /// <summary>
    /// Gets or sets the identifier of the current shop.
    /// </summary>
    public Guid Id { get; set; }
}