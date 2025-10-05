using Microsoft.EntityFrameworkCore;
using HotelCatalog.Domain.Entities;
using HotelCatalog.Dal.Configurations;

namespace HotelCatalog.Dal.Data
{
    public class HotelCatalogDbContext : DbContext
    {
        public HotelCatalogDbContext(DbContextOptions<HotelCatalogDbContext> options)
            : base(options)
        {
        }

        public DbSet<DiscountCategory> DiscountCategories { get; set; }
        public DbSet<RoomCategory> RoomCategories { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<RoomService> RoomServices { get; set; }
        public DbSet<ClientDiscount> ClientDiscounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new DiscountCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new RoomCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceConfiguration());
            modelBuilder.ApplyConfiguration(new RoomServiceConfiguration());
            modelBuilder.ApplyConfiguration(new ClientDiscountConfiguration());
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiscountCategory>().HasData(
                new DiscountCategory { DiscountId = 1, Name = "Student", DiscountPercent = 10.00m },
                new DiscountCategory { DiscountId = 2, Name = "Senior", DiscountPercent = 15.00m },
                new DiscountCategory { DiscountId = 3, Name = "VIP", DiscountPercent = 20.00m }
            );

            modelBuilder.Entity<RoomCategory>().HasData(
                new RoomCategory { CategoryId = 1, Name = "Standard", Description = "Standard room with basic amenities" },
                new RoomCategory { CategoryId = 2, Name = "Deluxe", Description = "Deluxe room with premium amenities" },
                new RoomCategory { CategoryId = 3, Name = "Suite", Description = "Luxury suite with extra space" }
            );

            modelBuilder.Entity<Service>().HasData(
                new Service { ServiceId = 1, Name = "WiFi", Description = "High-speed internet", Price = 0.00m },
                new Service { ServiceId = 2, Name = "Breakfast", Description = "Continental breakfast", Price = 150.00m },
                new Service { ServiceId = 3, Name = "Parking", Description = "Underground parking", Price = 100.00m },
                new Service { ServiceId = 4, Name = "Spa", Description = "Spa and wellness center", Price = 500.00m },
                new Service { ServiceId = 5, Name = "Pool", Description = "Swimming pool access", Price = 200.00m }
            );

            modelBuilder.Entity<RoomService>().HasData(
                new RoomService { CategoryId = 1, ServiceId = 1 }, // WiFi
                new RoomService { CategoryId = 1, ServiceId = 3 }, // Parking
                new RoomService { CategoryId = 2, ServiceId = 1 }, // WiFi
                new RoomService { CategoryId = 2, ServiceId = 2 }, // Breakfast
                new RoomService { CategoryId = 2, ServiceId = 3 }, // Parking
                new RoomService { CategoryId = 2, ServiceId = 5 }, // Pool
                new RoomService { CategoryId = 3, ServiceId = 1 }, // WiFi
                new RoomService { CategoryId = 3, ServiceId = 2 }, // Breakfast
                new RoomService { CategoryId = 3, ServiceId = 3 }, // Parking
                new RoomService { CategoryId = 3, ServiceId = 4 }, // Spa
                new RoomService { CategoryId = 3, ServiceId = 5 }  // Pool
            );
        }
    }
}