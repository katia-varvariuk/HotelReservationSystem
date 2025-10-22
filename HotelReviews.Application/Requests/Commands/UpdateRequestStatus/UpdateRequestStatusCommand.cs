using HotelReviews.Application.Common.Interfaces;
using HotelReviews.Application.DTOs;

namespace HotelReviews.Application.Requests.Commands.UpdateRequestStatus;
public record UpdateRequestStatusCommand(
    string Id,
    string NewStatus
) : ICommand<RequestDto>;