using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SweetShop.Domain.Entities;

namespace SweetShop.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the persistence mapping for the <see cref="Cart"/> entity.
/// </summary>
public sealed class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    /// <summary>
    /// Configures the <see cref="Cart"/> entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Carts");

        builder.HasKey(cart => cart.Id);

        builder.Property(cart => cart.CustomerId)
            .IsRequired();

        builder.Property(cart => cart.CreatedAt)
            .IsRequired();

        builder.Property(cart => cart.UpdatedAt)
            .IsRequired();

        builder.HasIndex(cart => cart.CustomerId)
            .IsUnique();

        builder.HasOne<Customer>()
            .WithOne()
            .HasForeignKey<Cart>(cart => cart.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(cart => cart.Items)
            .WithOne()
            .HasForeignKey(item => item.CartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}