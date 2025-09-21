using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using HotelReservation.Dal.Interfaces;

namespace HotelReservation.Dal.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly string _connectionString;
        private readonly ILogger<UnitOfWork> _logger;
        private NpgsqlConnection? _connection;
        private NpgsqlTransaction? _transaction;
        private bool _disposed;

        private IClientRepository? _clients;
        private IReservationRepository? _reservations;
        private IRoomRepository? _rooms;
        private IPaymentRepository? _payments;

        public UnitOfWork(IConfiguration configuration, ILogger<UnitOfWork> logger)
        {
            _connectionString = configuration.GetConnectionString("HotelReservationsDB") ??
                throw new ArgumentNullException(nameof(configuration), "Connection string not found");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IClientRepository Clients
        {
            get
            {
                EnsureConnection();
                return _clients ??= new ClientRepository(_connection!, _transaction, _logger.CreateLogger<ClientRepository>());
            }
        }

        public IReservationRepository Reservations
        {
            get
            {
                EnsureConnection();
                return _reservations ??= new ReservationRepository(_connection!, _transaction, _logger.CreateLogger<ReservationRepository>());
            }
        }

        public IRoomRepository Rooms
        {
            get
            {
                EnsureConnection();
                return _rooms ??= new RoomRepository(_connection!, _transaction, _logger.CreateLogger<RoomRepository>());
            }
        }

        public IPaymentRepository Payments
        {
            get
            {
                EnsureConnection();
                return _payments ??= new PaymentRepository(_connection!, _transaction, _logger.CreateLogger<PaymentRepository>());
            }
        }

        private void EnsureConnection()
        {
            if (_connection == null)
            {
                _connection = new NpgsqlConnection(_connectionString);
                _connection.Open();
            }
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("Transaction already started");
            }

            if (_connection == null)
            {
                _connection = new NpgsqlConnection(_connectionString);
                await _connection.OpenAsync(cancellationToken);
            }

            _transaction = await _connection.BeginTransactionAsync(cancellationToken);
            _logger.LogInformation("Transaction started");
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction to commit");
            }

            try
            {
                await _transaction.CommitAsync(cancellationToken);
                _logger.LogInformation("Transaction committed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while committing transaction");
                await RollbackAsync(cancellationToken);
                throw;
            }
            finally
            {
                await CleanupTransactionAsync();
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction to rollback");
            }

            try
            {
                await _transaction.RollbackAsync(cancellationToken);
                _logger.LogInformation("Transaction rolled back");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while rolling back transaction");
                throw;
            }
            finally
            {
                await CleanupTransactionAsync();
            }
        }

        private async Task CleanupTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }

            // Залишаємо connection відкритим для подальшого використання
            // Він буде закритий при Dispose
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Якщо є активна транзакція - комітимо її
            if (_transaction != null)
            {
                await CommitAsync(cancellationToken);
            }
            // Якщо транзакції немає, можливо варто кинути exception
            // або автоматично створювати транзакцію
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
                try
                {
                    _transaction?.Dispose();
                    _connection?.Close();
                    _connection?.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while disposing UnitOfWork");
                }
                finally
                {
                    _disposed = true;
                }
            }
        }
    }
}