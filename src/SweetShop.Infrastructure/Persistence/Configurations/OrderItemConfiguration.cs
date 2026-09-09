using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SweetShop.Domain.Entities;

namespace SweetShop.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the persistence mapping for the <see cref="OrderItem"/> entity.
/// </summary>
public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{

    /// <summary>
    /// Configures the <see cref="OrderItem"/> entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("OrderItems");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.OrderId)
            .IsRequired();

        builder.Property(item => item.ProductId)
            .IsRequired();

        builder.Property(item => item.ProductVariantId)
            .IsRequired();

        builder.Property(item => item.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(item => item.VariantName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(item => item.Quantity)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(item => item.Unit)
            .IsRequired();

        builder.ComplexProperty(
            item => item.UnitPrice,
            moneyBuilder =>
            {
                moneyBuilder.Property(money => money.Amount)
                    .HasColumnName("UnitPriceAmount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                moneyBuilder.Property(money => money.Currency)
                    .HasColumnName("UnitPriceCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

        builder.ComplexProperty(
            item => item.LineTotal,
            moneyBuilder =>
            {
                moneyBuilder.Property(money => money.Amount)
                    .HasColumnName("LineTotalAmount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                moneyBuilder.Property(money => money.Currency)
                    .HasColumnName("LineTotalCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

        builder.Property(item => item.CreatedAt)
            .IsRequired();

        builder.HasIndex(item => item.OrderId);

        builder.HasIndex(item => item.ProductVariantId);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ProductVariant>()
            .WithMany()
            .HasForeignKey(item => item.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}