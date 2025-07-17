using AutoMapper;
using MediatR;
using TicketService.Application.DTOs;
using TicketService.Application.Interfaces;

namespace TicketService.Application.CQRS.Tag.Queries;

public class GetTicketsByTagIdQuery : IRequest<IEnumerable<TicketDto>>
{
    public string TagId { get; set; }

    public GetTicketsByTagIdQuery(string tagId)
    {
        TagId = tagId;
    }
}

public class GetTicketsByTagIdQueryHandler : IRequestHandler<GetTicketsByTagIdQuery, IEnumerable<TicketDto>>
{
    private readonly ITicketService _ticketService;
    private readonly IMapper _mapper;

    public GetTicketsByTagIdQueryHandler(ITicketService ticketService, IMapper mapper)
    {
        _ticketService = ticketService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TicketDto>> Handle(GetTicketsByTagIdQuery request, CancellationToken cancellationToken)
    {
        var tickets = await _ticketService.GetTicketsByTagIdAsync(request.TagId, cancellationToken);
        return _mapper.Map<IEnumerable<TicketDto>>(tickets);
    }
}