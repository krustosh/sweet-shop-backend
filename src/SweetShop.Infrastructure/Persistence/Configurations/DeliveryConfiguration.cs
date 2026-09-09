using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SweetShop.Domain.Entities;

namespace SweetShop.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the persistence mapping for the <see cref="Delivery"/> entity.
/// </summary>
public sealed class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
    /// <summary>
    /// Configures the <see cref="Delivery"/> entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Deliveries");

        builder.HasKey(delivery => delivery.Id);

        builder.Property(delivery => delivery.OrderId)
            .IsRequired();

        builder.Property(delivery => delivery.DeliveryPersonId);

        builder.Property(delivery => delivery.Status)
            .IsRequired();

        builder.Property(delivery => delivery.AssignedAt);
        builder.Property(delivery => delivery.PickedUpAt);
        builder.Property(delivery => delivery.DeliveredAt);
        builder.Property(delivery => delivery.FailedAt);

        builder.Property(delivery => delivery.FailureReason)
            .HasMaxLength(1000);

        builder.Property(delivery => delivery.DeliveryNote)
            .HasMaxLength(2000);

        builder.Property(delivery => delivery.CreatedAt)
            .IsRequired();

        builder.Property(delivery => delivery.UpdatedAt)
            .IsRequired();

        builder.HasOne<Order>()
            .WithOne()
            .HasForeignKey<Delivery>(delivery => delivery.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<DeliveryPerson>()
            .WithMany()
            .HasForeignKey(delivery => delivery.DeliveryPersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(delivery => delivery.OrderId)
            .IsUnique();

        builder.HasIndex(delivery => delivery.DeliveryPersonId);
    }
}