using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCatalog.Domain.Entities;

namespace HotelCatalog.Dal.Configurations
{
    public class DiscountCategoryConfiguration : IEntityTypeConfiguration<DiscountCategory>
    {
        public void Configure(EntityTypeBuilder<DiscountCategory> builder)
        {
            builder.ToTable("discountcategories");

            builder.HasKey(d => d.DiscountId);
            builder.Property(d => d.DiscountId).HasColumnName("discountid");

            builder.Property(d => d.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(d => d.DiscountPercent)
                .HasColumnName("discountpercent")
                .HasColumnType("numeric(5,2)")
                .IsRequired();

            builder.HasIndex(d => d.Name).IsUnique();
        }
    }
}