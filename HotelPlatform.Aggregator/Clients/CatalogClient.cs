using HotelPlatform.Aggregator.DTOs;
using System.Text.Json;

namespace HotelPlatform.Aggregator.Clients;

public class CatalogClient : ICatalogClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CatalogClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public CatalogClient(HttpClient httpClient, ILogger<CatalogClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<RoomCategoryDto?> GetRoomCategoryAsync(
        int roomId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching category for room {RoomId}", roomId);

            var response = await _httpClient.GetAsync(
                $"/api/Catalog/room/{roomId}",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Failed to fetch category for room {RoomId}. Status: {StatusCode}",
                    roomId,
                    response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var category = JsonSerializer.Deserialize<RoomCategoryDto>(content, _jsonOptions);

            _logger.LogInformation(
                "Successfully fetched category for room {RoomId}",
                roomId);

            return category;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching category for room {RoomId}", roomId);
            return null;
        }
    }
}