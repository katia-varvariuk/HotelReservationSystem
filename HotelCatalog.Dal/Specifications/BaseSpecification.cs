using Ardalis.Specification;

namespace HotelCatalog.Dal.Specifications
{
    public abstract class BaseSpecification<T> : Specification<T> where T : class
    {
    }
}