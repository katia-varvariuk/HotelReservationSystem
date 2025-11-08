using HotelPlatform.Aggregator.DTOs;
using System.Text.Json;

namespace HotelPlatform.Aggregator.Clients;

public class ReservationsClient : IReservationsClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ReservationsClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ReservationsClient(HttpClient httpClient, ILogger<ReservationsClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<ReservationDto>> GetReservationsByRoomAsync(
        int roomId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching reservations for room {RoomId}", roomId);

            var response = await _httpClient.GetAsync(
                $"/api/Reservations/room/{roomId}",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Failed to fetch reservations for room {RoomId}. Status: {StatusCode}",
                    roomId,
                    response.StatusCode);
                return new List<ReservationDto>();
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var reservations = JsonSerializer.Deserialize<List<ReservationDto>>(content, _jsonOptions);

            _logger.LogInformation(
                "Successfully fetched {Count} reservations for room {RoomId}",
                reservations?.Count ?? 0,
                roomId);

            return reservations ?? new List<ReservationDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching reservations for room {RoomId}", roomId);
            return new List<ReservationDto>();
        }
    }
}