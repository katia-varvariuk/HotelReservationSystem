using HotelReviews.Application.Common.Interfaces; 
using HotelReviews.Domain.Exceptions;
using HotelReviews.Domain.Interfaces;
using MediatR;

namespace HotelReviews.Application.Reviews.Commands.DeleteReview;

public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ICacheService _cacheService; 

    public DeleteReviewCommandHandler(
        IReviewRepository reviewRepository,
        ICacheService cacheService) 
    {
        _reviewRepository = reviewRepository;
        _cacheService = cacheService; 
    }

    public async Task Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);

        if (review == null)
        {
            throw new NotFoundException("Review", request.Id);
        }

        await _reviewRepository.DeleteAsync(request.Id, cancellationToken);

        await _cacheService.RemoveAsync($"review:id:{request.Id}", cancellationToken);
        await _cacheService.RemoveByPrefixAsync($"reviews:room:{review.RoomId}", cancellationToken);
    }
}