using AutoMapper;
using HotelReviews.Application.DTOs;
using HotelReviews.Domain.Entities;
using HotelReviews.Domain.Interfaces;
using MediatR;

namespace HotelReviews.Application.Reviews.Commands.CreateReview;
public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ReviewDto>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public CreateReviewCommandHandler(IReviewRepository reviewRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = Review.Create(
            request.ClientId,
            request.RoomId,
            request.Rating,
            request.Comment);

        var createdReview = await _reviewRepository.CreateAsync(review, cancellationToken);

        return _mapper.Map<ReviewDto>(createdReview);
    }
}