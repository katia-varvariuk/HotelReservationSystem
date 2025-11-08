using HotelPlatform.Aggregator.DTOs;
using System.Text.Json;

namespace HotelPlatform.Aggregator.Clients;

public class ReviewsClient : IReviewsClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ReviewsClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ReviewsClient(HttpClient httpClient, ILogger<ReviewsClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<ReviewDto>> GetReviewsByRoomAsync(
        int roomId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching reviews for room {RoomId}", roomId);

            var response = await _httpClient.GetAsync(
                $"/api/Reviews/room/{roomId}",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Failed to fetch reviews for room {RoomId}. Status: {StatusCode}",
                    roomId,
                    response.StatusCode);
                return new List<ReviewDto>();
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var pagedResult = JsonSerializer.Deserialize<PagedResult<ReviewDto>>(content, _jsonOptions);

            _logger.LogInformation(
                "Successfully fetched {Count} reviews for room {RoomId}",
                pagedResult?.Items?.Count ?? 0,
                roomId);

            return pagedResult?.Items ?? new List<ReviewDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching reviews for room {RoomId}", roomId);
            return new List<ReviewDto>();
        }
    }
}
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}