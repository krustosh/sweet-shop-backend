using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SweetShop.Domain.Entities;

namespace SweetShop.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the persistence mapping for the <see cref="Product"/> entity.
/// </summary>
public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    /// <summary>
    /// Configures the <see cref="Product"/> entity.
/// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Products");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.ShopId)
            .IsRequired();

        builder.Property(product => product.CategoryId)
            .IsRequired();

        builder.Property(product => product.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(product => product.Description)
            .HasMaxLength(2000);

        builder.Property(product => product.DisplayOrder)
            .IsRequired();

        builder.Property(product => product.Status)
            .IsRequired();

        builder.Property(product => product.CreatedAt)
            .IsRequired();

        builder.Property(product => product.UpdatedAt)
            .IsRequired();

        builder.HasIndex(product => new
            {
                product.ShopId,
                product.CategoryId,
                product.DisplayOrder
            });
       
        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Shop>()
            .WithMany()
            .HasForeignKey(product => product.ShopId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}