namespace SweetShop.Application.DTOs;

/// <summary>
/// Represents the common identifier returned by application DTOs.
/// </summary>
public abstract record EntityDto
{
    /// <summary>
    /// Gets the entity identifier.
    /// </summary>
    public Guid Id { get; init; }
}