using AutoMapper;
using HotelReviews.Application.DTOs;
using HotelReviews.Domain.Exceptions;
using HotelReviews.Domain.Interfaces;
using MediatR;

namespace HotelReviews.Application.Reviews.Commands.ApproveReview;
public class ApproveReviewCommandHandler : IRequestHandler<ApproveReviewCommand, ReviewDto>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public ApproveReviewCommandHandler(IReviewRepository reviewRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<ReviewDto> Handle(ApproveReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);

        if (review == null)
        {
            throw new NotFoundException(nameof(review), request.Id);
        }

        review.Approve();

        await _reviewRepository.UpdateAsync(review, cancellationToken);

        return _mapper.Map<ReviewDto>(review);
    }
}