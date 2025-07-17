using AutoMapper;
using MediatR;
using TicketService.Application.DTOs;
using TicketService.Application.Interfaces;

namespace TicketService.Application.CQRS.Tag.Queries;

public class GetTicketByIdQuery : IRequest<TicketDto>
{
    public string TicketId { get; set; }

    public GetTicketByIdQuery(string ticketId)
    {
        TicketId = ticketId;
    }
}

public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, TicketDto>
{
    private readonly ITicketService _ticketService;
    private readonly IMapper _mapper;

    public GetTicketByIdQueryHandler(ITicketService ticketService, IMapper mapper)
    {
        _ticketService = ticketService;
        _mapper = mapper;
    }

    public async Task<TicketDto> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(request.TicketId, cancellationToken);
        return _mapper.Map<TicketDto>(ticket);
    }
}