using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SweetShop.Domain.Entities;

namespace SweetShop.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the persistence mapping for the <see cref="Inventory"/> entity.
/// </summary>
public sealed class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    /// <summary>
    /// Configures the <see cref="Inventory"/> entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Inventory");

        builder.HasKey(inventory => inventory.Id);

        builder.Property(inventory => inventory.ProductVariantId)
            .IsRequired();

        builder.Property(inventory => inventory.AvailableQuantity)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(inventory => inventory.ReservedQuantity)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(inventory => inventory.LowStockThreshold)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(inventory => inventory.CreatedAt)
            .IsRequired();

        builder.Property(inventory => inventory.UpdatedAt)
            .IsRequired();

        builder.HasOne<ProductVariant>()
            .WithOne()
            .HasForeignKey<Inventory>(inventory => inventory.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(inventory => inventory.ProductVariantId)
            .IsUnique();
    }
}