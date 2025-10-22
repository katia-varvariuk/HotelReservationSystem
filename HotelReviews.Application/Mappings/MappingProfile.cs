using AutoMapper;
using HotelReviews.Application.DTOs;
using HotelReviews.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HotelReviews.Application.Mappings;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating.Value));

        CreateMap<CreateReviewDto, Review>()
            .ConstructUsing(src => Review.Create(
                src.ClientId,
                src.RoomId,
                src.Rating,
                src.Comment));

        CreateMap<Request, RequestDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Value));

        CreateMap<CreateRequestDto, Request>()
            .ConstructUsing(src => Request.Create(
                src.ClientId,
                src.RoomId,
                src.RequestText,
                src.Category,
                src.Priority));
    }
}