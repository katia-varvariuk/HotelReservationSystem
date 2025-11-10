using AutoMapper;
using HotelReviews.Application.Common.Interfaces;
using HotelReviews.Application.Common.Models;
using HotelReviews.Application.DTOs;
using HotelReviews.Domain.Interfaces;
using MediatR;

namespace HotelReviews.Application.Reviews.Queries.GetReviewsByRoom;

public class GetReviewsByRoomQueryHandler : IRequestHandler<GetReviewsByRoomQuery, PagedResult<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;
    private readonly ITwoLevelCacheService _cacheService; 

    public GetReviewsByRoomQueryHandler(
        IReviewRepository reviewRepository,
        IMapper mapper,
        ITwoLevelCacheService cacheService)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<PagedResult<ReviewDto>> Handle(GetReviewsByRoomQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"reviews:room:{request.RoomId}:page:{request.Page}:size:{request.PageSize}:rating:{request.MinRating}:verified:{request.IsVerified}:approved:{request.IsApproved}";

        var cachedResult = await _cacheService.GetAsync<PagedResult<ReviewDto>>(cacheKey, cancellationToken);
        if (cachedResult != null)
        {
            return cachedResult;
        }

        var (reviews, totalCount) = await _reviewRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            roomId: request.RoomId,
            minRating: request.MinRating,
            isVerified: request.IsVerified,
            isApproved: request.IsApproved,
            cancellationToken: cancellationToken);

        var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        var result = PagedResult<ReviewDto>.Create(reviewDtos, totalCount, request.Page, request.PageSize);

        await _cacheService.SetAsync(cacheKey, result,
            memoryExpiration: TimeSpan.FromMinutes(2),
            redisExpiration: TimeSpan.FromMinutes(10),
            cancellationToken);

        return result;
    }
}