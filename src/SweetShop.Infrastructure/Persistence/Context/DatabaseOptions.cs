namespace SweetShop.Infrastructure.Persistence.Context;

/// <summary>
/// Represents database configuration for the Sweet Shop platform.
/// </summary>
public sealed class DatabaseOptions
{
    /// <summary>
    /// Gets the configuration section name.
    /// </summary>
    public const string SectionName = "Database";

    /// <summary>
    /// Gets or sets the SQL Server connection string.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}