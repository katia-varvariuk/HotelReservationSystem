using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCatalog.Domain.Entities;

namespace HotelCatalog.Dal.Configurations
{
    public class RoomCategoryConfiguration : IEntityTypeConfiguration<RoomCategory>
    {
        public void Configure(EntityTypeBuilder<RoomCategory> builder)
        {
            builder.ToTable("roomcategories");

            builder.HasKey(r => r.CategoryId);
            builder.Property(r => r.CategoryId).HasColumnName("categoryid");

            builder.Property(r => r.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(r => r.Description)
                .HasColumnName("description")
                .HasColumnType("text");

            builder.HasIndex(r => r.Name);
        }
    }
}