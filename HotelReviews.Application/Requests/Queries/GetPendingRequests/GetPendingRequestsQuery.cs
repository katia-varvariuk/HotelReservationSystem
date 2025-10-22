using HotelReviews.Application.Common.Interfaces;
using HotelReviews.Application.Common.Models;
using HotelReviews.Application.DTOs;

namespace HotelReviews.Application.Requests.Queries.GetPendingRequests;
public record GetPendingRequestsQuery(
    int Page = 1,
    int PageSize = 10
) : IQuery<PagedResult<RequestDto>>;