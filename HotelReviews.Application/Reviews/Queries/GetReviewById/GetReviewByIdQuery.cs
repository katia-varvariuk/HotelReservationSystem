using HotelReviews.Application.Common.Interfaces;
using HotelReviews.Application.DTOs;

namespace HotelReviews.Application.Reviews.Queries.GetReviewById;
public record GetReviewByIdQuery(string Id) : IQuery<ReviewDto?>;