using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCatalog.Domain.Entities;

namespace HotelCatalog.Dal.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.ToTable("services");

            builder.HasKey(s => s.ServiceId);
            builder.Property(s => s.ServiceId).HasColumnName("serviceid");

            builder.Property(s => s.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(s => s.Description)
                .HasColumnName("description")
                .HasColumnType("text");

            builder.Property(s => s.Price)
                .HasColumnName("price")
                .HasColumnType("numeric(10,2)")
                .IsRequired();

            builder.HasIndex(s => s.Name);
        }
    }
}