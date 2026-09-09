using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SweetShop.Domain.Entities;

namespace SweetShop.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the persistence mapping for the <see cref="DeliveryPerson"/> entity.
/// </summary>
public sealed class DeliveryPersonConfiguration : IEntityTypeConfiguration<DeliveryPerson>
{
    /// <summary>
    /// Configures the <see cref="DeliveryPerson"/> entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<DeliveryPerson> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("DeliveryPersons");

        builder.HasKey(deliveryPerson => deliveryPerson.Id);

        builder.Property(deliveryPerson => deliveryPerson.UserId)
            .IsRequired();

        builder.Property(deliveryPerson => deliveryPerson.Status)
            .IsRequired();

        builder.Property(deliveryPerson => deliveryPerson.CreatedAt)
            .IsRequired();

        builder.Property(deliveryPerson => deliveryPerson.UpdatedAt)
            .IsRequired();

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<DeliveryPerson>(
                deliveryPerson => deliveryPerson.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(deliveryPerson => deliveryPerson.UserId)
            .IsUnique();
    }
}