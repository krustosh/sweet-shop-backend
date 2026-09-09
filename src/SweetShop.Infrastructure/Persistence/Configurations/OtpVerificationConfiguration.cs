using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SweetShop.Domain.Entities;

namespace SweetShop.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the persistence mapping for the <see cref="OtpVerification"/> entity.
/// </summary>
public sealed class OtpVerificationConfiguration
    : IEntityTypeConfiguration<OtpVerification>
{
    /// <summary>
    /// Configures the <see cref="OtpVerification"/> entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<OtpVerification> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("OtpVerifications");

        builder.HasKey(otp => otp.Id);

        builder.Property(otp => otp.UserId)
            .IsRequired();

        builder.Property(otp => otp.MobileNumber)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(otp => otp.CodeHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(otp => otp.ExpiresAt)
            .IsRequired();

        builder.Property(otp => otp.AttemptCount)
            .IsRequired();

        builder.Property(otp => otp.MaxAttempts)
            .IsRequired();

        builder.Property(otp => otp.Status)
            .IsRequired();

        builder.Property(otp => otp.VerifiedAt);

        builder.Property(otp => otp.CreatedAt)
            .IsRequired();

        builder.Property(otp => otp.UpdatedAt)
            .IsRequired();

        builder.HasIndex(otp => new
        {
            otp.UserId,
            otp.Status,
            otp.ExpiresAt
        });

        builder.HasIndex(otp => new
        {
            otp.MobileNumber,
            otp.Status,
            otp.ExpiresAt
        });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(otp => otp.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}