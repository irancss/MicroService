using AutoMapper;
using MediatR;
using TicketService.Application.DTOs;
using TicketService.Application.Interfaces;

namespace TicketService.Application.CQRS.Tag.Queries;

public class SearchTicketsRequestQuery : IRequest<IEnumerable<TicketDto>>
{
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public SearchTicketsRequestQuery(string? searchTerm, string? sortBy, bool sortDescending, int pageNumber, int pageSize)
    {
        SearchTerm = searchTerm;
        SortBy = sortBy;
        SortDescending = sortDescending;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}

public class SearchTicketsRequestQueryHandler : IRequestHandler<SearchTicketsRequestQuery, IEnumerable<TicketDto>>
{
    private readonly ITicketService _ticketService;
    private readonly IMapper _mapper;

    public SearchTicketsRequestQueryHandler(ITicketService ticketService, IMapper mapper)
    {
        _ticketService = ticketService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TicketDto>> Handle(SearchTicketsRequestQuery request, CancellationToken cancellationToken)
    {
        var tickets = await _ticketService.SearchTicketsAsync(request.SearchTerm, request.SortBy, request.SortDescending, request.PageNumber, request.PageSize, cancellationToken);
        return _mapper.Map<IEnumerable<TicketDto>>(tickets);
    }
}