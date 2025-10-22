using AutoMapper;
using HotelReviews.Application.DTOs;
using HotelReviews.Domain.Exceptions;
using HotelReviews.Domain.Interfaces;
using MediatR;

namespace HotelReviews.Application.Requests.Commands.UpdateRequestStatus;
public class UpdateRequestStatusCommandHandler : IRequestHandler<UpdateRequestStatusCommand, RequestDto>
{
    private readonly IRequestRepository _requestRepository;
    private readonly IMapper _mapper;

    public UpdateRequestStatusCommandHandler(IRequestRepository requestRepository, IMapper mapper)
    {
        _requestRepository = requestRepository;
        _mapper = mapper;
    }

    public async Task<RequestDto> Handle(UpdateRequestStatusCommand request, CancellationToken cancellationToken)
    {
        var existingRequest = await _requestRepository.GetByIdAsync(request.Id, cancellationToken);

        if (existingRequest == null)
        {
            throw new NotFoundException(nameof(existingRequest), request.Id);
        }

        existingRequest.ChangeStatus(request.NewStatus);

        await _requestRepository.UpdateAsync(existingRequest, cancellationToken);

        return _mapper.Map<RequestDto>(existingRequest);
    }
}