using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SweetShop.Domain.Entities;

namespace SweetShop.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the persistence mapping for the <see cref="InventoryTransaction"/> entity.
/// </summary>
public sealed class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
    /// <summary>
    /// Configures the <see cref="InventoryTransaction"/> entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("InventoryTransactions");

        builder.HasKey(transaction => transaction.Id);

        builder.Property(transaction => transaction.InventoryId)
            .IsRequired();

        builder.Property(transaction => transaction.Type)
            .IsRequired();

        builder.Property(transaction => transaction.Quantity)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(transaction => transaction.PreviousQuantity)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(transaction => transaction.NewQuantity)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(transaction => transaction.ReferenceType)
            .HasMaxLength(100);

        builder.Property(transaction => transaction.ReferenceId);

        builder.Property(transaction => transaction.Reason)
            .HasMaxLength(1000);

        builder.Property(transaction => transaction.CreatedBy)
            .IsRequired();

        builder.Property(transaction => transaction.CreatedAt)
            .IsRequired();

        builder.HasOne<Inventory>()
            .WithMany()
            .HasForeignKey(transaction => transaction.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(transaction => new
        {
            transaction.InventoryId,
            transaction.CreatedAt
        });
    }
}