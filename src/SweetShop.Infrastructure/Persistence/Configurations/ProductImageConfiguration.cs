using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SweetShop.Domain.Entities;

namespace SweetShop.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the persistence mapping for the <see cref="ProductImage"/> entity.
/// </summary>
public sealed class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    /// <summary>
    /// Configures the <see cref="ProductImage"/> entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("ProductImages");

        builder.HasKey(image => image.Id);

        builder.Property(image => image.ProductId)
            .IsRequired();

        builder.Property(image => image.Url)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(image => image.AltText)
            .HasMaxLength(500);

        builder.Property(image => image.DisplayOrder)
            .IsRequired();

        builder.Property(image => image.IsPrimary)
            .IsRequired();

        builder.Property(image => image.CreatedAt)
            .IsRequired();

        builder.HasIndex(image => new
        {
            image.ProductId,
            image.DisplayOrder
        });

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(image => image.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}