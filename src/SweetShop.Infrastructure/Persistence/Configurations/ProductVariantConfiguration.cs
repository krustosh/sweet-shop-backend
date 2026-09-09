using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SweetShop.Domain.Entities;

namespace SweetShop.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the persistence mapping for the <see cref="ProductVariant"/> entity.
/// </summary>
public sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    /// <summary>
    /// Configures the <see cref="ProductVariant"/> entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("ProductVariants");

        builder.HasKey(variant => variant.Id);

        builder.Property(variant => variant.ProductId)
            .IsRequired();

        builder.Property(variant => variant.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.ComplexProperty(
            variant => variant.Quantity,
            quantityBuilder =>
            {
                quantityBuilder.Property(quantity => quantity.Value)
                    .HasColumnName("QuantityValue")
                    .HasPrecision(18, 3)
                    .IsRequired();

                quantityBuilder.Property(quantity => quantity.Unit)
                    .HasColumnName("QuantityUnit")
                    .IsRequired();
            });

        builder.ComplexProperty(
            variant => variant.Price,
            moneyBuilder =>
            {
                moneyBuilder.Property(money => money.Amount)
                    .HasColumnName("PriceAmount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                moneyBuilder.Property(money => money.Currency)
                    .HasColumnName("PriceCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

        builder.Property(variant => variant.DisplayOrder)
            .IsRequired();

        builder.Property(variant => variant.Status)
            .IsRequired();

        builder.Property(variant => variant.CreatedAt)
            .IsRequired();

        builder.Property(variant => variant.UpdatedAt)
            .IsRequired();

        builder.HasIndex(variant => new
        {
            variant.ProductId,
            variant.DisplayOrder
        });
        
        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(variant => variant.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}