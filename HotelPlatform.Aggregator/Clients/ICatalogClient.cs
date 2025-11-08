using HotelPlatform.Aggregator.DTOs;

namespace HotelPlatform.Aggregator.Clients;

public interface ICatalogClient
{
    Task<RoomCategoryDto?> GetRoomCategoryAsync(int roomId, CancellationToken cancellationToken = default);
}