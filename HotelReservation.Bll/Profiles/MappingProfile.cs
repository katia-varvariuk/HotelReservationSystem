using AutoMapper;
using HotelReservation.Domain.Entities;
using HotelReservation.Bll.DTOs;

namespace HotelReservation.Bll.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Client mappings
            CreateMap<Client, ClientDto>();
            CreateMap<ClientDto, Client>();
            CreateMap<CreateClientDto, Client>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ClientId, opt => opt.Ignore())
                .ForMember(dest => dest.Reservations, opt => opt.Ignore());
            CreateMap<UpdateClientDto, Client>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Reservations, opt => opt.Ignore());

            // Room mappings
            CreateMap<Room, RoomDto>();
            CreateMap<RoomDto, Room>();
            CreateMap<CreateRoomDto, Room>()
                .ForMember(dest => dest.RoomId, opt => opt.Ignore())
                .ForMember(dest => dest.Reservations, opt => opt.Ignore());
            CreateMap<Room, AvailableRoomDto>()
                .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => true)); // будемо встановлювати динамічно

            // Reservation mappings
            CreateMap<Reservation, ReservationDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
            CreateMap<ReservationDto, Reservation>();
            CreateMap<CreateReservationDto, Reservation>()
                .ForMember(dest => dest.ReservationId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ReservationStatus.Pending))
                .ForMember(dest => dest.Client, opt => opt.Ignore())
                .ForMember(dest => dest.Room, opt => opt.Ignore())
                .ForMember(dest => dest.Payments, opt => opt.Ignore());

            // Payment mappings
            CreateMap<Payment, PaymentDto>();
            CreateMap<PaymentDto, Payment>();
            CreateMap<CreatePaymentDto, Payment>()
                .ForMember(dest => dest.PaymentId, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Reservation, opt => opt.Ignore());
        }
    }
}