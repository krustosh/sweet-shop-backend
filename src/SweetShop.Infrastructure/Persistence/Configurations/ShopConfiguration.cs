using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SweetShop.Domain.Entities;

namespace SweetShop.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the persistence mapping for the <see cref="Shop"/> entity.
/// </summary>
public sealed class ShopConfiguration : IEntityTypeConfiguration<Shop>
{
    /// <summary>
    /// Configures the <see cref="Shop"/> entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Shop> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Shops");

        builder.HasKey(shop => shop.Id);

        builder.Property(shop => shop.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(shop => shop.Description)
            .HasMaxLength(2000);

        builder.Property(shop => shop.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(shop => shop.Email)
            .HasMaxLength(320);

        builder.Property(shop => shop.Address)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(shop => shop.LogoUrl)
            .HasMaxLength(2048);

        builder.Property(shop => shop.Status)
            .IsRequired();

        builder.Property(shop => shop.CreatedAt)
            .IsRequired();

        builder.Property(shop => shop.UpdatedAt)
            .IsRequired();
    }
}