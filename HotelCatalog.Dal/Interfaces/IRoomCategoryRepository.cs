using HotelCatalog.Domain.Entities;

namespace HotelCatalog.Dal.Interfaces
{
    public interface IRoomCategoryRepository : IRepository<RoomCategory>
    {
        Task<RoomCategory?> GetCategoryByIdAsync(int categoryId, CancellationToken cancellationToken = default);
        Task LoadServicesAsync(RoomCategory category, CancellationToken cancellationToken = default);

        Task<IEnumerable<Service>> GetServicesByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<RoomCategory>> GetCategoriesWithServiceCountAsync(CancellationToken cancellationToken = default);
    }
}