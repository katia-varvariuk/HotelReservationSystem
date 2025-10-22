using HotelReviews.Application.Common.Interfaces;
using HotelReviews.Application.DTOs;

namespace HotelReviews.Application.Requests.Commands.CreateRequest;
public record CreateRequestCommand(
    int ClientId,
    int RoomId,
    string RequestText,
    string Category = "general",
    int Priority = 3
) : ICommand<RequestDto>;