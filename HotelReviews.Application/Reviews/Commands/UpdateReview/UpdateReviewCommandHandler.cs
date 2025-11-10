using AutoMapper;
using HotelReviews.Application.Common.Interfaces; 
using HotelReviews.Application.DTOs;
using HotelReviews.Domain.Exceptions;
using HotelReviews.Domain.Interfaces;
using MediatR;

namespace HotelReviews.Application.Reviews.Commands.UpdateReview;

public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, ReviewDto>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService; 

    public UpdateReviewCommandHandler(
        IReviewRepository reviewRepository,
        IMapper mapper,
        ICacheService cacheService) 
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
        _cacheService = cacheService; 
    }

    public async Task<ReviewDto> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);

        if (review == null)
        {
            throw new NotFoundException(nameof(review), request.Id);
        }

        review.Update(request.Rating, request.Comment);
        await _reviewRepository.UpdateAsync(review, cancellationToken);

        await _cacheService.RemoveAsync($"review:id:{request.Id}", cancellationToken);
        await _cacheService.RemoveByPrefixAsync($"reviews:room:{review.RoomId}", cancellationToken);

        return _mapper.Map<ReviewDto>(review);
    }
}