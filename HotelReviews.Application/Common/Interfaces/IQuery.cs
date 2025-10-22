using MediatR;

namespace HotelReviews.Application.Common.Interfaces;
public interface IQuery<out TResponse> : IRequest<TResponse>
{
}