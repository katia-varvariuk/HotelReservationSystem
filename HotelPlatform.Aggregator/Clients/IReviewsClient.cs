using HotelPlatform.Aggregator.DTOs;

namespace HotelPlatform.Aggregator.Clients;

public interface IReviewsClient
{
    Task<List<ReviewDto>> GetReviewsByRoomAsync(int roomId, CancellationToken cancellationToken = default);
}