using Microsoft.EntityFrameworkCore;
using HotelCatalog.Dal.Data;
using HotelCatalog.Dal.Interfaces;
using HotelCatalog.Domain.Entities;

namespace HotelCatalog.Dal.Repositories
{
    public class DiscountCategoryRepository : Repository<DiscountCategory>, IDiscountCategoryRepository
    {
        public DiscountCategoryRepository(HotelCatalogDbContext context) : base(context)
        {
        }

        public async Task<DiscountCategory?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(d => d.Name == name, cancellationToken);
        }

        public async Task<IEnumerable<DiscountCategory>> GetDiscountsAbovePercentAsync(decimal percent, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(d => d.DiscountPercent >= percent)
                .OrderByDescending(d => d.DiscountPercent)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(int discountId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(d => d.DiscountId == discountId, cancellationToken);
        }
    }
}