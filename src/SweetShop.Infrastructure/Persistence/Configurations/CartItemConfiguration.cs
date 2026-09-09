using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SweetShop.Domain.Entities;

namespace SweetShop.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the persistence mapping for the <see cref="CartItem"/> entity.
/// </summary>
public sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    /// <summary>
    /// Configures the <see cref="CartItem"/> entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("CartItems");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.CartId)
            .IsRequired();

        builder.Property(item => item.ProductVariantId)
            .IsRequired();

        builder.Property(item => item.Quantity)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(item => item.CreatedAt)
            .IsRequired();

        builder.Property(item => item.UpdatedAt)
            .IsRequired();

        builder.HasIndex(item => new
        {
            item.CartId,
            item.ProductVariantId
        })
        .IsUnique();

        builder.HasOne<ProductVariant>()
            .WithMany()
            .HasForeignKey(item => item.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}