using Microsoft.EntityFrameworkCore;
using HotelCatalog.Dal.Data;
using HotelCatalog.Dal.Interfaces;
using HotelCatalog.Domain.Entities;

namespace HotelCatalog.Dal.Repositories
{
    public class ServiceRepository : Repository<Service>, IServiceRepository
    {
        public ServiceRepository(HotelCatalogDbContext context) : base(context)
        {
        }

        public async Task<Service?> GetServiceWithRoomCategoriesAsync(int serviceId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(s => s.RoomServices)
                    .ThenInclude(rs => rs.RoomCategory)
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId, cancellationToken);
        }

        public async Task<IEnumerable<Service>> GetAllServicesWithRoomCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(s => s.RoomServices)
                    .ThenInclude(rs => rs.RoomCategory)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Service>> GetServicesByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(s => s.Price >= minPrice && s.Price <= maxPrice)
                .OrderBy(s => s.Price)
                .ToListAsync(cancellationToken);
        }
    }
}