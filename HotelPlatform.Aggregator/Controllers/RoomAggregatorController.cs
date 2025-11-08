using HotelPlatform.Aggregator.Clients;
using HotelPlatform.Aggregator.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HotelPlatform.Aggregator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomAggregatorController : ControllerBase
{
    private readonly IReviewsClient _reviewsClient;
    private readonly IReservationsClient _reservationsClient;
    private readonly ICatalogClient _catalogClient;
    private readonly ILogger<RoomAggregatorController> _logger;

    public RoomAggregatorController(
        IReviewsClient reviewsClient,
        IReservationsClient reservationsClient,
        ICatalogClient catalogClient,
        ILogger<RoomAggregatorController> logger)
    {
        _reviewsClient = reviewsClient;
        _reservationsClient = reservationsClient;
        _catalogClient = catalogClient;
        _logger = logger;
    }
    [HttpGet("room/{roomId}")]
    public async Task<ActionResult<RoomDetailsDto>> GetRoomDetails(
        int roomId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Aggregating data for room {RoomId}", roomId);

        // Паралельні запити до всіх сервісів
        var reviewsTask = _reviewsClient.GetReviewsByRoomAsync(roomId, cancellationToken);
        var reservationsTask = _reservationsClient.GetReservationsByRoomAsync(roomId, cancellationToken);
        var categoryTask = _catalogClient.GetRoomCategoryAsync(roomId, cancellationToken);
        await Task.WhenAll(reviewsTask, reservationsTask, categoryTask);

        var reviews = await reviewsTask;
        var reservations = await reservationsTask;
        var category = await categoryTask;

        // Обчислення середнього рейтингу
        var averageRating = reviews.Any()
            ? reviews.Average(r => r.Rating)
            : 0;

        var result = new RoomDetailsDto
        {
            RoomId = roomId,
            Category = category,
            Reviews = reviews,
            Reservations = reservations,
            AverageRating = Math.Round(averageRating, 2),
            TotalReviews = reviews.Count
        };

        _logger.LogInformation(
            "Successfully aggregated data for room {RoomId}: {ReviewCount} reviews, {ReservationCount} reservations",
            roomId,
            reviews.Count,
            reservations.Count);

        return Ok(result);
    }
}