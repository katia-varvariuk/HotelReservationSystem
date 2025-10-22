using AutoMapper;
using HotelReviews.Application.DTOs;
using HotelReviews.Domain.Entities;
using HotelReviews.Domain.Interfaces;
using MediatR;

namespace HotelReviews.Application.Requests.Commands.CreateRequest;
public class CreateRequestCommandHandler : IRequestHandler<CreateRequestCommand, RequestDto>
{
    private readonly IRequestRepository _requestRepository;
    private readonly IMapper _mapper;

    public CreateRequestCommandHandler(IRequestRepository requestRepository, IMapper mapper)
    {
        _requestRepository = requestRepository;
        _mapper = mapper;
    }

    public async Task<RequestDto> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
    {
        var newRequest = Request.Create(
            request.ClientId,
            request.RoomId,
            request.RequestText,
            request.Category,
            request.Priority);

        var createdRequest = await _requestRepository.CreateAsync(newRequest, cancellationToken);

        return _mapper.Map<RequestDto>(createdRequest);
    }
}