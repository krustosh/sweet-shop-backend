using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SweetShop.Domain.Entities;

namespace SweetShop.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the persistence mapping for the <see cref="Customer"/> entity.
/// </summary>
public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    /// <summary>
    /// Configures the <see cref="Customer"/> entity.
/// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Customers");

        builder.HasKey(customer => customer.Id);

        builder.Property(customer => customer.UserId)
            .IsRequired();

        builder.Property(customer => customer.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(customer => customer.Email)
            .HasMaxLength(320);

        builder.Property(customer => customer.Notes)
            .HasMaxLength(2000);

        builder.Property(customer => customer.CreatedAt)
            .IsRequired();

        builder.Property(customer => customer.UpdatedAt)
            .IsRequired();

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<Customer>(customer => customer.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(customer => customer.UserId)
            .IsUnique();
    }
}