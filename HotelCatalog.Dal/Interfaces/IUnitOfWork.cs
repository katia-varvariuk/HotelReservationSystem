using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelCatalog.Dal.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IServiceRepository Services { get; }
        IRoomCategoryRepository RoomCategories { get; }
        IDiscountCategoryRepository DiscountCategories { get; }

        IRepository<T> Repository<T>() where T : class;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}