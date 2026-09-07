using SweetShop.Domain.Common;
using SweetShop.Domain.Enums;


namespace SweetShop.Domain.Entities;

/// <summary>
/// Represents a delivery personnel profile.
/// </summary>
public sealed class DeliveryPerson : AuditableEntity
{
    /// <summary>
    /// Gets the identifier of the associated user.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the delivery person's status.
    /// </summary>
    public DeliveryPersonStatus Status { get; private set; }

    /// <summary>
    /// Initializes a new delivery person.
    /// </summary>
    /// <param name="userId">The identifier of the associated user.</param>
    public DeliveryPerson(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(userId));
        }

        UserId = userId;
        Status = DeliveryPersonStatus.Active;
    }

    /// <summary>
    /// Activates the delivery person.
    /// </summary>
    public void Activate()
    {
        Status = DeliveryPersonStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the delivery person.
    /// </summary>
    public void Deactivate()
    {
        Status = DeliveryPersonStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }
}