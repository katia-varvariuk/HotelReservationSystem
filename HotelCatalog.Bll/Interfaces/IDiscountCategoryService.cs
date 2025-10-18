using HotelCatalog.Bll.DTOs;

namespace HotelCatalog.Bll.Interfaces
{
    public interface IDiscountCategoryService
    {
        Task<DiscountCategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<DiscountCategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<DiscountCategoryDto>> GetDiscountsAbovePercentAsync(decimal percent, CancellationToken cancellationToken = default);
        Task<DiscountCategoryDto> CreateAsync(CreateDiscountCategoryDto dto, CancellationToken cancellationToken = default);
        Task<DiscountCategoryDto> UpdateAsync(UpdateDiscountCategoryDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}