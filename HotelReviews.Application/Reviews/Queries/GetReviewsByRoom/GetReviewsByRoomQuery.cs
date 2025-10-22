using HotelReviews.Application.Common.Interfaces;
using HotelReviews.Application.Common.Models;
using HotelReviews.Application.DTOs;

namespace HotelReviews.Application.Reviews.Queries.GetReviewsByRoom;
public record GetReviewsByRoomQuery(
    int RoomId,
    int Page = 1,
    int PageSize = 10,
    int? MinRating = null,
    bool? IsVerified = null,
    bool? IsApproved = null
) : IQuery<PagedResult<ReviewDto>>;