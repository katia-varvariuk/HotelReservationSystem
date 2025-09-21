using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using HotelReservation.Domain.Entities;
using HotelReservation.Dal.Interfaces;

namespace HotelReservation.Dal.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<ReservationRepository> _logger;

        public ReservationRepository(IConfiguration configuration, ILogger<ReservationRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("HotelReservationsDB") ??
                throw new ArgumentNullException("Connection string not found");
            _logger = logger;
        }

        public async Task<Reservation?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT reservationid, clientid, roomid, checkindate, checkoutdate, status 
                FROM reservations 
                WHERE reservationid = @Id";

            await using var connection = new NpgsqlConnection(_connectionString);

            return await connection.QueryFirstOrDefaultAsync<Reservation>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT reservationid, clientid, roomid, checkindate, checkoutdate, status 
                FROM reservations 
                ORDER BY checkindate DESC";

            await using var connection = new NpgsqlConnection(_connectionString);

            return await connection.QueryAsync<Reservation>(sql);
        }

        public async Task<int> CreateAsync(Reservation entity, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                INSERT INTO reservations (clientid, roomid, checkindate, checkoutdate, status)
                VALUES (@ClientId, @RoomId, @CheckInDate, @CheckOutDate, @Status)
                RETURNING reservationid";

            await using var connection = new NpgsqlConnection(_connectionString);

            return await connection.QuerySingleAsync<int>(sql, new
            {
                entity.ClientId,
                entity.RoomId,
                entity.CheckInDate,
                entity.CheckOutDate,
                Status = (int)entity.Status
            });
        }

        public async Task<bool> UpdateAsync(Reservation entity, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                UPDATE reservations 
                SET clientid = @ClientId, 
                    roomid = @RoomId, 
                    checkindate = @CheckInDate, 
                    checkoutdate = @CheckOutDate, 
                    status = @Status
                WHERE reservationid = @ReservationId";

            await using var connection = new NpgsqlConnection(_connectionString);

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                entity.ReservationId,
                entity.ClientId,
                entity.RoomId,
                entity.CheckInDate,
                entity.CheckOutDate,
                Status = (int)entity.Status
            });

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM reservations WHERE reservationid = @Id";

            await using var connection = new NpgsqlConnection(_connectionString);

            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Reservation>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT reservationid, clientid, roomid, checkindate, checkoutdate, status 
                FROM reservations 
                WHERE clientid = @ClientId
                ORDER BY checkindate DESC";

            await using var connection = new NpgsqlConnection(_connectionString);

            return await connection.QueryAsync<Reservation>(sql, new { ClientId = clientId });
        }

        public async Task<Reservation?> GetWithDetailsAsync(int reservationId, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT 
                    r.reservationid, r.clientid, r.roomid, r.checkindate, r.checkoutdate, r.status,
                    c.clientid, c.fullname, c.passportdata, c.phone, c.email, c.createdat,
                    rm.roomid, rm.roomnumber, rm.capacity, rm.price
                FROM reservations r
                INNER JOIN clients c ON r.clientid = c.clientid
                INNER JOIN rooms rm ON r.roomid = rm.roomid
                WHERE r.reservationid = @ReservationId";

            await using var connection = new NpgsqlConnection(_connectionString);

            var reservationDictionary = new Dictionary<int, Reservation>();

            var result = await connection.QueryAsync<Reservation, Client, Room, Reservation>(
                sql,
                (reservation, client, room) =>
                {
                    if (!reservationDictionary.TryGetValue(reservation.ReservationId, out var reservationEntry))
                    {
                        reservationEntry = reservation;
                        reservationEntry.Client = client;
                        reservationEntry.Room = room;
                        reservationDictionary.Add(reservationEntry.ReservationId, reservationEntry);
                    }

                    return reservationEntry;
                },
                new { ReservationId = reservationId },
                splitOn: "clientid,roomid");

            return result.FirstOrDefault();
        }

        public async Task<bool> UpdateStatusAsync(int reservationId, ReservationStatus status, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                UPDATE reservations 
                SET status = @Status
                WHERE reservationid = @ReservationId";

            await using var connection = new NpgsqlConnection(_connectionString);

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                ReservationId = reservationId,
                Status = (int)status
            });

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Reservation>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT reservationid, clientid, roomid, checkindate, checkoutdate, status 
                FROM reservations 
                WHERE checkindate >= @From AND checkoutdate <= @To
                ORDER BY checkindate";

            await using var connection = new NpgsqlConnection(_connectionString);

            return await connection.QueryAsync<Reservation>(sql, new { From = from, To = to });
        }
    }
}