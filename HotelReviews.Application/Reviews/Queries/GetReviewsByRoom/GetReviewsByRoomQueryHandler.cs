using AutoMapper;
using HotelReviews.Application.Common.Models;
using HotelReviews.Application.DTOs;
using HotelReviews.Domain.Interfaces;
using MediatR;

namespace HotelReviews.Application.Reviews.Queries.GetReviewsByRoom;
public class GetReviewsByRoomQueryHandler : IRequestHandler<GetReviewsByRoomQuery, PagedResult<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public GetReviewsByRoomQueryHandler(IReviewRepository reviewRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<ReviewDto>> Handle(GetReviewsByRoomQuery request, CancellationToken cancellationToken)
    {
        var (reviews, totalCount) = await _reviewRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            roomId: request.RoomId,
            minRating: request.MinRating,
            isVerified: request.IsVerified,
            isApproved: request.IsApproved,
            cancellationToken: cancellationToken);

        var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);

        return PagedResult<ReviewDto>.Create(reviewDtos, totalCount, request.Page, request.PageSize);
    }
}