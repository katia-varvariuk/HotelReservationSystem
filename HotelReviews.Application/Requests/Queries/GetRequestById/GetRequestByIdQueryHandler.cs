using AutoMapper;
using HotelReviews.Application.DTOs;
using HotelReviews.Domain.Interfaces;
using MediatR;

namespace HotelReviews.Application.Requests.Queries.GetRequestById;
public class GetRequestByIdQueryHandler : IRequestHandler<GetRequestByIdQuery, RequestDto?>
{
    private readonly IRequestRepository _requestRepository;
    private readonly IMapper _mapper;

    public GetRequestByIdQueryHandler(IRequestRepository requestRepository, IMapper mapper)
    {
        _requestRepository = requestRepository;
        _mapper = mapper;
    }

    public async Task<RequestDto?> Handle(GetRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var existingRequest = await _requestRepository.GetByIdAsync(request.Id, cancellationToken);

        return existingRequest == null ? null : _mapper.Map<RequestDto>(existingRequest);
    }
}