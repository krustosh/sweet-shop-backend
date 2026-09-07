namespace SweetShop.Domain.Common;

/// <summary>
/// Represents an event that has occurred within the domain.
/// </summary>
public abstract record DomainEvent
{
    /// <summary>
    /// Gets the date and time when the domain event occurred.
    /// </summary>
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}