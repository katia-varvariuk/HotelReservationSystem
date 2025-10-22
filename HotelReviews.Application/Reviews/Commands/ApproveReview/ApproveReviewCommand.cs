using HotelReviews.Application.Common.Interfaces;
using HotelReviews.Application.DTOs;

namespace HotelReviews.Application.Reviews.Commands.ApproveReview;
public record ApproveReviewCommand(string Id) : ICommand<ReviewDto>;