using HotelReviews.Application.Common.Interfaces;
using HotelReviews.Application.DTOs;

namespace HotelReviews.Application.Reviews.Commands.CreateReview;
public record CreateReviewCommand(
    int ClientId,
    int RoomId,
    int Rating,
    string Comment
) : ICommand<ReviewDto>;