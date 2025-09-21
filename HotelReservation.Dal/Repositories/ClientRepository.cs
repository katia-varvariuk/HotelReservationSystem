using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using HotelReservation.Domain.Entities;
using HotelReservation.Dal.Interfaces;

namespace HotelReservation.Dal.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<ClientRepository> _logger;

        public ClientRepository(IConfiguration configuration, ILogger<ClientRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("HotelReservationsDB") ??
                throw new ArgumentNullException("Connection string not found");
            _logger = logger;
        }

        public async Task<Client?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT clientid, fullname, passportdata, phone, email, createdat 
                FROM clients 
                WHERE clientid = @Id";

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            if (await reader.ReadAsync(cancellationToken))
            {
                return MapClientFromReader(reader);
            }

            return null;
        }

        public async Task<IEnumerable<Client>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT clientid, fullname, passportdata, phone, email, createdat 
                FROM clients 
                ORDER BY fullname";

            var clients = new List<Client>();

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new NpgsqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                clients.Add(MapClientFromReader(reader));
            }

            return clients;
        }

        public async Task<int> CreateAsync(Client entity, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                INSERT INTO clients (fullname, passportdata, phone, email, createdat)
                VALUES (@FullName, @PassportData, @Phone, @Email, @CreatedAt)
                RETURNING clientid";

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@FullName", entity.FullName);
            command.Parameters.AddWithValue("@PassportData", entity.PassportData);
            command.Parameters.AddWithValue("@Phone", entity.Phone);
            command.Parameters.AddWithValue("@Email", entity.Email);
            command.Parameters.AddWithValue("@CreatedAt", entity.CreatedAt);

            var result = await command.ExecuteScalarAsync(cancellationToken);
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(Client entity, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                UPDATE clients 
                SET fullname = @FullName, 
                    passportdata = @PassportData, 
                    phone = @Phone, 
                    email = @Email
                WHERE clientid = @ClientId";

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ClientId", entity.ClientId);
            command.Parameters.AddWithValue("@FullName", entity.FullName);
            command.Parameters.AddWithValue("@PassportData", entity.PassportData);
            command.Parameters.AddWithValue("@Phone", entity.Phone);
            command.Parameters.AddWithValue("@Email", entity.Email);

            var rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM clients WHERE clientid = @Id";

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
            return rowsAffected > 0;
        }

        public async Task<Client?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT clientid, fullname, passportdata, phone, email, createdat 
                FROM clients 
                WHERE email = @Email";

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            if (await reader.ReadAsync(cancellationToken))
            {
                return MapClientFromReader(reader);
            }

            return null;
        }

        public async Task<IEnumerable<Client>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT clientid, fullname, passportdata, phone, email, createdat 
                FROM clients 
                WHERE fullname ILIKE @SearchTerm OR email ILIKE @SearchTerm
                ORDER BY fullname";

            var clients = new List<Client>();

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                clients.Add(MapClientFromReader(reader));
            }

            return clients;
        }

        private static Client MapClientFromReader(IDataReader reader)
        {
            return new Client
            {
                ClientId = reader.GetInt32(reader.GetOrdinal("clientid")),
                FullName = reader.GetString(reader.GetOrdinal("fullname")),
                PassportData = reader.GetString(reader.GetOrdinal("passportdata")),
                Phone = reader.GetString(reader.GetOrdinal("phone")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdat"))
            };
        }
    }
}