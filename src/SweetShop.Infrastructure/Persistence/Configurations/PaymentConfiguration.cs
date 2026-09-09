using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SweetShop.Domain.Entities;

namespace SweetShop.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the persistence mapping for the <see cref="Payment"/> entity.
/// </summary>
public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    /// <summary>
    /// Configures the <see cref="Payment"/> entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Payments");

        builder.HasKey(payment => payment.Id);

        builder.Property(payment => payment.OrderId)
            .IsRequired();

        builder.Property(payment => payment.PaymentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(payment => payment.Method)
            .IsRequired();

        builder.Property(payment => payment.Status)
            .IsRequired();

        builder.ComplexProperty(
            payment => payment.Amount,
            moneyBuilder =>
            {
                moneyBuilder.Property(money => money.Amount)
                    .HasColumnName("Amount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                moneyBuilder.Property(money => money.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

        builder.Property(payment => payment.Provider)
            .HasMaxLength(100);

        builder.Property(payment => payment.ProviderTransactionId)
            .HasMaxLength(200);

        builder.Property(payment => payment.FailureCode)
            .HasMaxLength(100);

        builder.Property(payment => payment.FailureMessage)
            .HasMaxLength(1000);

        builder.Property(payment => payment.PaidAt);

        builder.Property(payment => payment.CreatedAt)
            .IsRequired();

        builder.Property(payment => payment.UpdatedAt)
            .IsRequired();

        builder.HasOne<Order>()
            .WithMany()
            .HasForeignKey(payment => payment.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(payment => payment.PaymentNumber)
            .IsUnique();

        builder.HasIndex(payment => payment.ProviderTransactionId)
            .IsUnique()
            .HasFilter("[ProviderTransactionId] IS NOT NULL");

        builder.HasIndex(payment => new
        {
            payment.OrderId,
            payment.CreatedAt
        });
    }
}