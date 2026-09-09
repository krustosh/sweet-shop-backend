using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SweetShop.Domain.Entities;

namespace SweetShop.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the persistence mapping for the <see cref="Address"/> entity.
/// </summary>
public sealed class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    /// <summary>
    /// Configures the <see cref="Address"/> entity.
/// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Addresses");

        builder.HasKey(address => address.Id);

        builder.Property(address => address.CustomerId)
            .IsRequired();

        builder.Property(address => address.Label)
            .IsRequired();

        builder.Property(address => address.RecipientName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(address => address.MobileNumber)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(address => address.AddressLine1)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(address => address.AddressLine2)
            .HasMaxLength(500);

        builder.Property(address => address.Landmark)
            .HasMaxLength(300);

        builder.Property(address => address.Area)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(address => address.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(address => address.State)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(address => address.PostalCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(address => address.Latitude)
            .HasPrecision(9, 6);

        builder.Property(address => address.Longitude)
            .HasPrecision(9, 6);

        builder.Property(address => address.IsDefault)
            .IsRequired();

        builder.Property(address => address.CreatedAt)
            .IsRequired();

        builder.Property(address => address.UpdatedAt)
            .IsRequired();

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(address => address.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(address => new
        {
            address.CustomerId,
            address.IsDefault
        });
    }
}