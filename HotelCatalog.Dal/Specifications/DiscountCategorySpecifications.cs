using Ardalis.Specification;
using HotelCatalog.Domain.Entities;

namespace HotelCatalog.Dal.Specifications
{
    public class DiscountsAbovePercentSpec : Specification<DiscountCategory>
    {
        public DiscountsAbovePercentSpec(decimal percent)
        {
            Query
                .Where(d => d.DiscountPercent >= percent)
                .OrderByDescending(d => d.DiscountPercent);
        }
    }

    public class DiscountByNameSpec : Specification<DiscountCategory>, ISingleResultSpecification<DiscountCategory>
    {
        public DiscountByNameSpec(string name)
        {
            Query.Where(d => d.Name == name);
        }
    }

    public class ActiveDiscountsSpec : Specification<DiscountCategory>
    {
        public ActiveDiscountsSpec()
        {
            Query
                .Where(d => d.DiscountPercent > 0)
                .OrderBy(d => d.Name);
        }
    }
}