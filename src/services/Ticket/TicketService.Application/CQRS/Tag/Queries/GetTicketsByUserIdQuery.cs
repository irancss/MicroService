using AutoMapper;
using MediatR;
using TicketService.Application.DTOs;
using TicketService.Application.Interfaces;

namespace TicketService.Application.CQRS.Tag.Queries;

public class GetTicketsByUserIdQuery : IRequest<IEnumerable<TicketDto>>
{
    public string UserId { get; set; }

    public GetTicketsByUserIdQuery(string userId)
    {
        UserId = userId;
    }
}
public class GetTicketsByUserIdQueryHandler : IRequestHandler<GetTicketsByUserIdQuery, IEnumerable<TicketDto>>
{
    private readonly ITicketService _ticketService;
    private readonly IMapper _mapper;

    public GetTicketsByUserIdQueryHandler(ITicketService ticketService, IMapper mapper)
    {
        _ticketService = ticketService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TicketDto>> Handle(GetTicketsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var tickets = await _ticketService.GetTicketsByUserIdAsync(request.UserId, cancellationToken);
        return _mapper.Map<IEnumerable<TicketDto>>(tickets);
    }
}