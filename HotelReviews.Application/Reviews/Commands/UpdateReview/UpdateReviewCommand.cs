using HotelReviews.Application.Common.Interfaces;
using HotelReviews.Application.DTOs;

namespace HotelReviews.Application.Reviews.Commands.UpdateReview;
public record UpdateReviewCommand(
    string Id,
    int Rating,
    string Comment
) : ICommand<ReviewDto>;