using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SweetShop.Domain.Entities;

namespace SweetShop.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the persistence mapping for the <see cref="Category"/> entity.
/// </summary>
public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    /// <summary>
    /// Configures the <see cref="Category"/> entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Categories");

        builder.HasKey(category => category.Id);

        builder.Property(category => category.ShopId)
            .IsRequired();

        builder.Property(category => category.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(category => category.Description)
            .HasMaxLength(2000);

        builder.Property(category => category.ImageUrl)
            .HasMaxLength(2048);

        builder.Property(category => category.DisplayOrder)
            .IsRequired();

        builder.Property(category => category.Status)
            .IsRequired();

        builder.Property(category => category.CreatedAt)
            .IsRequired();

        builder.Property(category => category.UpdatedAt)
            .IsRequired();
        builder.HasOne<Shop>()
            .WithMany()
            .HasForeignKey(category => category.ShopId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(category => new
        {
            category.ShopId,
            category.DisplayOrder   
        });
    }
}