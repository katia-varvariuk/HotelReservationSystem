using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCatalog.Domain.Entities;

namespace HotelCatalog.Dal.Configurations
{
    public class RoomServiceConfiguration : IEntityTypeConfiguration<RoomService>
    {
        public void Configure(EntityTypeBuilder<RoomService> builder)
        {
            builder.ToTable("roomservices");
            builder.HasKey(rs => new { rs.CategoryId, rs.ServiceId });

            builder.Property(rs => rs.CategoryId).HasColumnName("categoryid");
            builder.Property(rs => rs.ServiceId).HasColumnName("serviceid");
            builder.HasOne(rs => rs.RoomCategory)
                .WithMany(rc => rc.RoomServices)
                .HasForeignKey(rs => rs.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rs => rs.Service)
                .WithMany(s => s.RoomServices)
                .HasForeignKey(rs => rs.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}