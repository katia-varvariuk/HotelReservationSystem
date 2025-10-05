using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCatalog.Domain.Entities;

namespace HotelCatalog.Dal.Configurations
{
    public class ClientDiscountConfiguration : IEntityTypeConfiguration<ClientDiscount>
    {
        public void Configure(EntityTypeBuilder<ClientDiscount> builder)
        {
            builder.ToTable("clientdiscounts");
            builder.HasKey(cd => new { cd.ClientId, cd.DiscountId });

            builder.Property(cd => cd.ClientId).HasColumnName("clientid");
            builder.Property(cd => cd.DiscountId).HasColumnName("discountid");
            builder.HasOne(cd => cd.DiscountCategory)
                .WithMany(dc => dc.ClientDiscounts)
                .HasForeignKey(cd => cd.DiscountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}