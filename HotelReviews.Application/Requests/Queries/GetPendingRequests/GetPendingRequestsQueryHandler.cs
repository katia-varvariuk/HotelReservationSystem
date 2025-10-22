using AutoMapper;
using HotelReviews.Application.Common.Models;
using HotelReviews.Application.DTOs;
using HotelReviews.Domain.Interfaces;
using MediatR;

namespace HotelReviews.Application.Requests.Queries.GetPendingRequests;
public class GetPendingRequestsQueryHandler : IRequestHandler<GetPendingRequestsQuery, PagedResult<RequestDto>>
{
    private readonly IRequestRepository _requestRepository;
    private readonly IMapper _mapper;

    public GetPendingRequestsQueryHandler(IRequestRepository requestRepository, IMapper mapper)
    {
        _requestRepository = requestRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<RequestDto>> Handle(GetPendingRequestsQuery request, CancellationToken cancellationToken)
    {
        var (requests, totalCount) = await _requestRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            status: "pending",
            cancellationToken: cancellationToken);

        var requestDtos = _mapper.Map<IEnumerable<RequestDto>>(requests);

        return PagedResult<RequestDto>.Create(requestDtos, totalCount, request.Page, request.PageSize);
    }
}