using HotelReviews.Application.Common.Interfaces;
using HotelReviews.Application.DTOs;

namespace HotelReviews.Application.Requests.Queries.GetRequestById;
public record GetRequestByIdQuery(string Id) : IQuery<RequestDto?>;