using Ardalis.Specification;
using HotelCatalog.Domain.Entities;

namespace HotelCatalog.Dal.Specifications
{
    public class ServicesByPriceRangeSpec : Specification<Service>
    {
        public ServicesByPriceRangeSpec(decimal minPrice, decimal maxPrice)
        {
            Query
                .Where(s => s.Price >= minPrice && s.Price <= maxPrice)
                .OrderBy(s => s.Price);
        }
    }

    public class ServicesWithCategoriesSpec : Specification<Service>
    {
        public ServicesWithCategoriesSpec()
        {
            Query
                .Include(s => s.RoomServices)
                    .ThenInclude(rs => rs.RoomCategory);
        }
    }

    public class ServiceByIdWithCategoriesSpec : Specification<Service>, ISingleResultSpecification<Service>
    {
        public ServiceByIdWithCategoriesSpec(int serviceId)
        {
            Query
                .Where(s => s.ServiceId == serviceId)
                .Include(s => s.RoomServices)
                    .ThenInclude(rs => rs.RoomCategory);
        }
    }

    public class ServicesPaginatedSpec : Specification<Service>
    {
        public ServicesPaginatedSpec(
            int skip,
            int take,
            string? searchTerm = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? sortBy = null,
            bool sortDescending = false)
        {
            // Filtering
            if (!string.IsNullOrEmpty(searchTerm))
            {
                Query.Where(s =>
                    s.Name.Contains(searchTerm) ||
                    (s.Description != null && s.Description.Contains(searchTerm)));
            }

            if (minPrice.HasValue)
            {
                Query.Where(s => s.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                Query.Where(s => s.Price <= maxPrice.Value);
            }

            // Sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "name":
                        if (sortDescending)
                            Query.OrderByDescending(s => s.Name);
                        else
                            Query.OrderBy(s => s.Name);
                        break;
                    case "price":
                        if (sortDescending)
                            Query.OrderByDescending(s => s.Price);
                        else
                            Query.OrderBy(s => s.Price);
                        break;
                    default:
                        Query.OrderBy(s => s.ServiceId);
                        break;
                }
            }
            else
            {
                Query.OrderBy(s => s.ServiceId);
            }

            // Pagination
            Query
                .Skip(skip)
                .Take(take);
        }
    }

    public class ServicesCountSpec : Specification<Service>
    {
        public ServicesCountSpec(
            string? searchTerm = null,
            decimal? minPrice = null,
            decimal? maxPrice = null)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                Query.Where(s =>
                    s.Name.Contains(searchTerm) ||
                    (s.Description != null && s.Description.Contains(searchTerm)));
            }

            if (minPrice.HasValue)
            {
                Query.Where(s => s.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                Query.Where(s => s.Price <= maxPrice.Value);
            }
        }
    }
}