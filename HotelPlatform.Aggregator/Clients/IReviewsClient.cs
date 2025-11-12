using HotelPlatform.Aggregator.DTOs;
using System.Threading.Tasks;

namespace HotelPlatform.Aggregator.Clients;

public interface IReviewsClient
{
    Task<List<ReviewDto>> GetReviewsByRoomAsync(int roomId, CancellationToken cancellationToken = default);
}