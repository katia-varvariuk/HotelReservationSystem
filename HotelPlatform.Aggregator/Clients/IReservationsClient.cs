using HotelPlatform.Aggregator.DTOs;

namespace HotelPlatform.Aggregator.Clients;

public interface IReservationsClient
{
    Task<List<ReservationDto>> GetReservationsByRoomAsync(int roomId, CancellationToken cancellationToken = default);
}