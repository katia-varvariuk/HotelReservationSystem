using AutoMapper;
using HotelReviews.Application.Common.Interfaces; 
using HotelReviews.Application.DTOs;
using HotelReviews.Domain.Interfaces;
using MediatR;

namespace HotelReviews.Application.Reviews.Queries.GetReviewById;

public class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQuery, ReviewDto?>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService; 

    public GetReviewByIdQueryHandler(
        IReviewRepository reviewRepository,
        IMapper mapper,
        ICacheService cacheService) 
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
        _cacheService = cacheService; 
    }

    public async Task<ReviewDto?> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"review:id:{request.Id}";
        var cachedReview = await _cacheService.GetAsync<ReviewDto>(cacheKey, cancellationToken);

        if (cachedReview != null)
        {
            return cachedReview;
        }

        var review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);

        if (review == null)
        {
            return null;
        }

        var reviewDto = _mapper.Map<ReviewDto>(review);

        await _cacheService.SetAsync(cacheKey, reviewDto, TimeSpan.FromMinutes(10), cancellationToken);

        return reviewDto;
    }
}