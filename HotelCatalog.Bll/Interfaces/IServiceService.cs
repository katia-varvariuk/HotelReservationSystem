using HotelCatalog.Bll.DTOs;

namespace HotelCatalog.Bll.Interfaces
{
    public interface IServiceService
    {
        Task<ServiceDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ServiceDto?> GetWithRoomCategoriesAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ServiceDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<ServiceDto>> GetAllWithRoomCategoriesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<ServiceDto>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
        Task<ServiceDto> CreateAsync(CreateServiceDto dto, CancellationToken cancellationToken = default);
        Task<ServiceDto> UpdateAsync(UpdateServiceDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResult<ServiceDto>> GetPagedAsync(ServiceQueryParameters parameters, CancellationToken cancellationToken = default);
        Task<PagedResult<ServiceDto>> GetPagedWithSpecificationAsync(ServiceQueryParameters parameters, CancellationToken cancellationToken = default);
        Task<IEnumerable<ServiceDto>> GetByPriceRangeWithSpecificationAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
    }
}