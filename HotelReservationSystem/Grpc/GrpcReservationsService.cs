using Grpc.Core;
using HotelReservation.Api.Grpc;
using HotelReservation.Bll.Services;
using HotelReservation.Bll.DTOs;
using HotelReservation.Domain.Entities;

namespace HotelReservation.Api.Grpc;

public class GrpcReservationsService : ReservationsService.ReservationsServiceBase
{
    private readonly IReservationService _reservationService;
    private readonly ILogger<GrpcReservationsService> _logger;

    public GrpcReservationsService(
        IReservationService reservationService,
        ILogger<GrpcReservationsService> logger)
    {
        _reservationService = reservationService;
        _logger = logger;
    }

    public override async Task<GetAllReservationsResponse> GetAllReservations(
        GetAllReservationsRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("gRPC GetAllReservations: Page={Page}", request.Page);

        var reservations = await _reservationService.GetAllAsync(context.CancellationToken);

        var response = new GetAllReservationsResponse
        {
            TotalCount = reservations.Count()
        };

        response.Items.AddRange(reservations.Select(MapToGrpc));

        return response;
    }

    public override async Task<ReservationResponse> GetReservationById(
        GetReservationByIdRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("gRPC GetReservationById: Id={ReservationId}", request.Id);

        var reservation = await _reservationService.GetByIdAsync(request.Id, context.CancellationToken);

        if (reservation == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Reservation with ID '{request.Id}' not found"));
        }

        return new ReservationResponse
        {
            Reservation = MapToGrpc(reservation)
        };
    }

    public override async Task<GetAllReservationsResponse> GetReservationsByHotel(
        GetReservationsByHotelRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("gRPC GetReservationsByHotel: HotelId={HotelId}", request.HotelId);

        var reservations = await _reservationService.GetAllAsync(context.CancellationToken);

        var response = new GetAllReservationsResponse
        {
            TotalCount = reservations.Count()
        };

        response.Items.AddRange(reservations.Select(MapToGrpc));

        return response;
    }

    public override async Task<ReservationResponse> CreateReservation(
        CreateReservationRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("gRPC CreateReservation: ClientId={ClientId}, RoomId={RoomId}",
            request.ClientId, request.RoomId);

        var dto = new CreateReservationDto
        {
            ClientId = request.ClientId,
            RoomId = request.RoomId,
            CheckInDate = DateTime.Parse(request.CheckInDate),
            CheckOutDate = DateTime.Parse(request.CheckOutDate)
        };

        var created = await _reservationService.CreateAsync(dto, context.CancellationToken);

        return new ReservationResponse
        {
            Reservation = MapToGrpc(created)
        };
    }

    private static ReservationMessage MapToGrpc(ReservationDto dto)
    {
        double totalPrice = 0.0;
        if (dto.Payments != null && dto.Payments.Any())
        {
            try
            {
                totalPrice = dto.Payments.Sum(p => (double)p.Amount);
            }
            catch
            {
                totalPrice = 0.0;
            }
        }

        return new ReservationMessage
        {
            Id = dto.ReservationId,
            HotelId = 0,  
            ClientId = dto.ClientId,
            RoomId = dto.RoomId,
            CheckInDate = dto.CheckInDate.ToString("O"),
            CheckOutDate = dto.CheckOutDate.ToString("O"),
            TotalPrice = totalPrice,
            Status = dto.Status.ToString(),
            CreatedAt = DateTime.UtcNow.ToString("O")
        };
    }
}