using Microsoft.EntityFrameworkCore.Storage;
using HotelCatalog.Dal.Data;
using HotelCatalog.Dal.Interfaces;

namespace HotelCatalog.Dal.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HotelCatalogDbContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed;
        private IServiceRepository? _services;
        private IRoomCategoryRepository? _roomCategories;
        private IDiscountCategoryRepository? _discountCategories;

        private readonly Dictionary<Type, object> _repositories;

        public UnitOfWork(HotelCatalogDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
        }

        public IServiceRepository Services
        {
            get
            {
                return _services ??= new ServiceRepository(_context);
            }
        }

        public IRoomCategoryRepository RoomCategories
        {
            get
            {
                return _roomCategories ??= new RoomCategoryRepository(_context);
            }
        }

        public IDiscountCategoryRepository DiscountCategories
        {
            get
            {
                return _discountCategories ??= new DiscountCategoryRepository(_context);
            }
        }

        public IRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);

            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new Repository<T>(_context);
            }

            return (IRepository<T>)_repositories[type];
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);

                if (_transaction != null)
                {
                    await _transaction.CommitAsync(cancellationToken);
                }
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
                _disposed = true;
            }
        }
    }
}