using AutoMapper;
using HotelCatalog.Domain.Entities;
using HotelCatalog.Bll.DTOs;

namespace HotelCatalog.Bll.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
                CreateMap<Service, ServiceDto>()
            .ForMember(dest => dest.RoomCategories, opt => opt.MapFrom(src =>
                src.RoomServices.Select(rs => rs.RoomCategory)))
            .MaxDepth(2); 

            CreateMap<RoomCategory, RoomCategoryDto>()
                .ForMember(dest => dest.Services, opt => opt.MapFrom(src =>
                    src.RoomServices.Select(rs => rs.Service)))
                .ForMember(dest => dest.ServiceCount, opt => opt.MapFrom(src =>
                    src.RoomServices.Count))
                .MaxDepth(2); 

            CreateMap<Service, ServiceDto>()
                .ForMember(dest => dest.RoomCategories, opt => opt.MapFrom(src =>
                    src.RoomServices.Select(rs => rs.RoomCategory)));

            CreateMap<CreateServiceDto, Service>()
                .ForMember(dest => dest.ServiceId, opt => opt.Ignore())
                .ForMember(dest => dest.RoomServices, opt => opt.Ignore());

            CreateMap<UpdateServiceDto, Service>()
                .ForMember(dest => dest.RoomServices, opt => opt.Ignore());

            CreateMap<RoomCategory, RoomCategoryDto>()
                .ForMember(dest => dest.Services, opt => opt.MapFrom(src =>
                    src.RoomServices.Select(rs => rs.Service)))
                .ForMember(dest => dest.ServiceCount, opt => opt.MapFrom(src =>
                    src.RoomServices.Count));

            CreateMap<CreateRoomCategoryDto, RoomCategory>()
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.RoomServices, opt => opt.Ignore());

            CreateMap<UpdateRoomCategoryDto, RoomCategory>()
                .ForMember(dest => dest.RoomServices, opt => opt.Ignore());

            CreateMap<DiscountCategory, DiscountCategoryDto>();

            CreateMap<CreateDiscountCategoryDto, DiscountCategory>()
                .ForMember(dest => dest.DiscountId, opt => opt.Ignore())
                .ForMember(dest => dest.ClientDiscounts, opt => opt.Ignore());

            CreateMap<UpdateDiscountCategoryDto, DiscountCategory>()
                .ForMember(dest => dest.ClientDiscounts, opt => opt.Ignore());
        }
    }
}