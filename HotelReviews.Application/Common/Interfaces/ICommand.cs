using HotelReviews.Domain.Entities;
using MediatR;

namespace HotelReviews.Application.Common.Interfaces;

public interface ICommand : IRequest
{
}
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}