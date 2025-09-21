using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using HotelReservation.Domain.Entities;
using HotelReservation.Dal.Interfaces;

namespace HotelReservation.Dal.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<RoomRepository> _logger;

        public RoomRepository(IConfiguration configuration, ILogger<RoomRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("HotelReservationsDB") ??
                throw new ArgumentNullException("Connection string not found");
            _logger = logger;
        }

        public async Task<Room?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT roomid, roomnumber, capacity, price 
                FROM rooms 
                WHERE roomid = @Id";

            await using var connection = new NpgsqlConnection(_connectionString);

            return await connection.QueryFirstOrDefaultAsync<Room>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Room>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT roomid, roomnumber, capacity, price 
                FROM rooms 
                ORDER BY roomnumber";

            await using var connection = new NpgsqlConnection(_connectionString);

            return await connection.QueryAsync<Room>(sql);
        }

        public async Task<int> CreateAsync(Room entity, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                INSERT INTO rooms (roomnumber, capacity, price)
                VALUES (@RoomNumber, @Capacity, @Price)
                RETURNING roomid";

            await using var connection = new NpgsqlConnection(_connectionString);

            return await connection.QuerySingleAsync<int>(sql, new
            {
                entity.RoomNumber,
                entity.Capacity,
                entity.Price
            });
        }

        public async Task<bool> UpdateAsync(Room entity, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                UPDATE rooms 
                SET roomnumber = @RoomNumber, 
                    capacity = @Capacity, 
                    price = @Price
                WHERE roomid = @RoomId";

            await using var connection = new NpgsqlConnection(_connectionString);

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                entity.RoomId,
                entity.RoomNumber,
                entity.Capacity,
                entity.Price
            });

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM rooms WHERE roomid = @Id";

            await using var connection = new NpgsqlConnection(_connectionString);

            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT r.roomid, r.roomnumber, r.capacity, r.price 
                FROM rooms r
                WHERE r.roomid NOT IN (
                    SELECT res.roomid 
                    FROM reservations res 
                    WHERE res.status NOT IN (3, 4) -- Not CheckedOut or Cancelled
                    AND (
                        (res.checkindate <= @CheckOut AND res.checkoutdate >= @CheckIn)
                    )
                )
                ORDER BY r.roomnumber";

            await using var connection = new NpgsqlConnection(_connectionString);

            return await connection.QueryAsync<Room>(sql, new
            {
                CheckIn = checkIn,
                CheckOut = checkOut
            });
        }

        public async Task<Room?> GetByRoomNumberAsync(string roomNumber, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT roomid, roomnumber, capacity, price 
                FROM rooms 
                WHERE roomnumber = @RoomNumber";

            await using var connection = new NpgsqlConnection(_connectionString);

            return await connection.QueryFirstOrDefaultAsync<Room>(sql, new { RoomNumber = roomNumber });
        }
    }
}