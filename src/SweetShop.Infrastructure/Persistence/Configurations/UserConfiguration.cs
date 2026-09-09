using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SweetShop.Domain.Entities;

namespace SweetShop.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the persistence mapping for the <see cref="User"/> entity.
/// </summary>
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <summary>
    /// Configures the <see cref="User"/> entity.
/// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.MobileNumber)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(user => user.Role)
            .IsRequired();

        builder.Property(user => user.Status)
            .IsRequired();

        builder.Property(user => user.LastLoginAt);

        builder.Property(user => user.CreatedAt)
            .IsRequired();

        builder.Property(user => user.UpdatedAt)
            .IsRequired();

        builder.HasIndex(user => user.MobileNumber)
            .IsUnique();
    }
}