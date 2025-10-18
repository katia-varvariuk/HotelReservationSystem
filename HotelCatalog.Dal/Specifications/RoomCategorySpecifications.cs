using Ardalis.Specification;
using HotelCatalog.Domain.Entities;

namespace HotelCatalog.Dal.Specifications
{
    public class CategoriesWithServiceCountSpec : Specification<RoomCategory>
    {
        public CategoriesWithServiceCountSpec()
        {
            Query
                .Include(rc => rc.RoomServices)
                .OrderByDescending(rc => rc.RoomServices.Count);
        }
    }

    public class CategoryByIdWithServicesSpec : Specification<RoomCategory>, ISingleResultSpecification<RoomCategory>
    {
        public CategoryByIdWithServicesSpec(int categoryId)
        {
            Query
                .Where(rc => rc.CategoryId == categoryId)
                .Include(rc => rc.RoomServices)
                    .ThenInclude(rs => rs.Service);
        }
    }

    public class CategoriesByNameSpec : Specification<RoomCategory>
    {
        public CategoriesByNameSpec(string searchTerm)
        {
            Query
                .Where(rc => rc.Name.Contains(searchTerm) ||
                            (rc.Description != null && rc.Description.Contains(searchTerm)))
                .OrderBy(rc => rc.Name);
        }
    }
}