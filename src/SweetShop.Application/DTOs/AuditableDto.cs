namespace SweetShop.Application.DTOs;

/// <summary>
/// Represents common audit information returned by application DTOs.
/// </summary>
public abstract record AuditableDto : EntityDto
{
    /// <summary>
    /// Gets the date and time when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when the entity was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; init; }
}