namespace SweetShop.Domain.Common;

/// <summary>
/// Provides creation and modification timestamps for domain entities.
/// </summary>
public abstract class AuditableEntity : Entity
{
    /// <summary>
    /// Gets the date and time when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// Gets the date and time when the entity was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; protected set; }

    /// <summary>
    /// Initializes a new auditable entity with the current UTC timestamp.
    /// </summary>
    protected AuditableEntity()
    {
        var now = DateTime.UtcNow;

        CreatedAt = now;
        UpdatedAt = now;
    }

    /// <summary>
    /// Initializes an existing auditable entity.
    /// </summary>
    /// <param name="id">The existing entity identifier.</param>
    /// <param name="createdAt">The entity creation timestamp.</param>
    /// <param name="updatedAt">The entity modification timestamp.</param>
    protected AuditableEntity(
        Guid id,
        DateTime createdAt,
        DateTime updatedAt)
        : base(id)
    {
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}