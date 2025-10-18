using HotelCatalog.Domain.Entities;

namespace HotelCatalog.Dal.Interfaces
{
    public interface IServiceRepository : IRepository<Service>
    {
        
        Task<Service?> GetServiceWithRoomCategoriesAsync(int serviceId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Service>> GetAllServicesWithRoomCategoriesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Service>> GetServicesByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
    }
}