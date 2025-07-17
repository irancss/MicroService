using AutoMapper;
using MediatR;
using TicketService.Application.DTOs;
using TicketService.Application.Interfaces;

namespace TicketService.Application.CQRS.Tag.Queries;

public class GetTicketsByStatusQuery : IRequest<IEnumerable<TicketDto>>
{
    public string Status { get; set; }

    public GetTicketsByStatusQuery(string status)
    {
        Status = status;
    }
}

public class GetTicketsByStatusQueryHandler : IRequestHandler<GetTicketsByStatusQuery, IEnumerable<TicketDto>>
{
    private readonly ITicketService _ticketService;
    private readonly IMapper _mapper;

    public GetTicketsByStatusQueryHandler(ITicketService ticketService, IMapper mapper)
    {
        _ticketService = ticketService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TicketDto>> Handle(GetTicketsByStatusQuery request, CancellationToken cancellationToken)
    {
        var tickets = await _ticketService.GetTicketsByStatusAsync(request.Status, cancellationToken);
        return _mapper.Map<IEnumerable<TicketDto>>(tickets);
    }
}