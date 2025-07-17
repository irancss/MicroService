using AutoMapper;
using MediatR;
using TicketService.Application.DTOs;
using TicketService.Application.Interfaces;

namespace TicketService.Application.CQRS.Tag.Queries;

public class GetTicketsByPriorityQuery : IRequest<IEnumerable<TicketDto>>
{
    public string Priority { get; set; }

    public GetTicketsByPriorityQuery(string priority)
    {
        Priority = priority;
    }
}
public class GetTicketsByPriorityQueryHandler : IRequestHandler<GetTicketsByPriorityQuery, IEnumerable<TicketDto>>
{
    private readonly ITicketService _ticketService;
    private readonly IMapper _mapper;

    public GetTicketsByPriorityQueryHandler(ITicketService ticketService, IMapper mapper)
    {
        _ticketService = ticketService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TicketDto>> Handle(GetTicketsByPriorityQuery request, CancellationToken cancellationToken)
    {
        var tickets = await _ticketService.GetTicketsByPriorityAsync(request.Priority, cancellationToken);
        return _mapper.Map<IEnumerable<TicketDto>>(tickets);
    }
}