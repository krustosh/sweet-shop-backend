using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SweetShop.Domain.Entities;

namespace SweetShop.Infrastructure.Persistence.Configurations;

/// <summary>
/// Represents the configuration for the <see cref="Order"/> entity in the database.
/// </summary>
public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    /// <summary>
    /// Configures the <see cref="Order"/> entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Orders");

        builder.HasKey(order => order.Id);

        builder.Property(order => order.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(order => order.CustomerId)
            .IsRequired();

        builder.Property(order => order.FulfillmentType)
            .IsRequired();

        builder.Property(order => order.Status)
            .IsRequired();

        builder.Property(order => order.PaymentStatus)
            .IsRequired();

        builder.ComplexProperty(
            order => order.Subtotal,
            moneyBuilder =>
            {
                moneyBuilder.Property(money => money.Amount)
                    .HasColumnName("SubtotalAmount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                moneyBuilder.Property(money => money.Currency)
                    .HasColumnName("SubtotalCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

        builder.ComplexProperty(
            order => order.DeliveryFee,
            moneyBuilder =>
            {
                moneyBuilder.Property(money => money.Amount)
                    .HasColumnName("DeliveryFeeAmount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                moneyBuilder.Property(money => money.Currency)
                    .HasColumnName("DeliveryFeeCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

        builder.ComplexProperty(
            order => order.DiscountAmount,
            moneyBuilder =>
            {
                moneyBuilder.Property(money => money.Amount)
                    .HasColumnName("DiscountAmount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                moneyBuilder.Property(money => money.Currency)
                    .HasColumnName("DiscountCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

        builder.ComplexProperty(
            order => order.TotalAmount,
            moneyBuilder =>
            {
                moneyBuilder.Property(money => money.Amount)
                    .HasColumnName("TotalAmount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                moneyBuilder.Property(money => money.Currency)
                    .HasColumnName("TotalCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

        builder.Property(order => order.CustomerNote)
            .HasMaxLength(2000);

        builder.ComplexProperty(
            order => order.DeliveryAddressSnapshot,
            addressBuilder =>
            {
                addressBuilder.Property(address => address.RecipientName)
                    .HasMaxLength(200)
                    .IsRequired();

                addressBuilder.Property(address => address.MobileNumber)
                    .HasMaxLength(10)
                    .IsRequired();

                addressBuilder.Property(address => address.AddressLine1)
                    .HasMaxLength(500)
                    .IsRequired();

                addressBuilder.Property(address => address.AddressLine2)
                    .HasMaxLength(500);

                addressBuilder.Property(address => address.Landmark)
                    .HasMaxLength(300);

                addressBuilder.Property(address => address.Area)
                    .HasMaxLength(200)
                    .IsRequired();

                addressBuilder.Property(address => address.City)
                    .HasMaxLength(100)
                    .IsRequired();

                addressBuilder.Property(address => address.State)
                    .HasMaxLength(100)
                    .IsRequired();

                addressBuilder.Property(address => address.PostalCode)
                    .HasMaxLength(10)
                    .IsRequired();

                addressBuilder.Property(address => address.Latitude)
                    .HasPrecision(9, 6);

                addressBuilder.Property(address => address.Longitude)
                    .HasPrecision(9, 6);
            });

        builder.Property(order => order.PlacedAt);

        builder.Property(order => order.AcceptedAt);

        builder.Property(order => order.PreparingAt);

        builder.Property(order => order.ReadyAt);

        builder.Property(order => order.CompletedAt);

        builder.Property(order => order.CancelledAt);

        builder.Property(order => order.CancellationReason)
            .HasMaxLength(1000);

        builder.Property(order => order.CreatedAt)
            .IsRequired();

        builder.Property(order => order.UpdatedAt)
            .IsRequired();

        builder.HasIndex(order => order.OrderNumber)
            .IsUnique();

        builder.HasIndex(order => new
        {
            order.CustomerId,
            order.CreatedAt
        });

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(order => order.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(order => order.Items)
            .WithOne()
            .HasForeignKey(item => item.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
