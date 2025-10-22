using HotelReviews.Application.Common.Interfaces;

namespace HotelReviews.Application.Reviews.Commands.DeleteReview;
public record DeleteReviewCommand(string Id) : ICommand;