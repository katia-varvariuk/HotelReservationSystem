using HotelCatalog.Domain.Entities;

namespace HotelCatalog.Dal.Interfaces
{
    public interface IDiscountCategoryRepository : IRepository<DiscountCategory>
    {
        Task<DiscountCategory?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<DiscountCategory>> GetDiscountsAbovePercentAsync(decimal percent, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int discountId, CancellationToken cancellationToken = default);
    }
}