using HotelReviews.Domain.Exceptions;
using HotelReviews.Domain.Interfaces;
using MediatR;

namespace HotelReviews.Application.Reviews.Commands.DeleteReview;
public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand>
{
    private readonly IReviewRepository _reviewRepository;

    public DeleteReviewCommandHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var exists = await _reviewRepository.ExistsAsync(request.Id, cancellationToken);

        if (!exists)
        {
            throw new NotFoundException("Review", request.Id);
        }

        await _reviewRepository.DeleteAsync(request.Id, cancellationToken);
    }
}