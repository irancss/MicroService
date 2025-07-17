using AutoMapper;
using MediatR;
using TicketService.Application.DTOs;
using TicketService.Application.Interfaces;

namespace TicketService.Application.CQRS.Tag.Commands;

public class UpdateTicketRequestCommand : IRequest
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }

    public UpdateTicketRequestCommand(string Id, string title, string description, string status, string priority)
    {
        Id = Id;
        Title = title;
        Description = description;
        Status = status;
        Priority = priority;
    }
}

public class UpdateTicketRequestCommandHandler : IRequestHandler<UpdateTicketRequestCommand>
{
    private readonly ITicketService _ticketService;
    private readonly IMapper _mapper;

    public UpdateTicketRequestCommandHandler(ITicketService ticketService, IMapper mapper)
    {
        _ticketService = ticketService;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateTicketRequestCommand request, CancellationToken cancellationToken)
    {
        var map = _mapper.Map<UpdateTicketRequestCommand, TicketDto>(request);
         await _ticketService.UpdateTicketAsync(map);
         return Unit.Value;
    }
}
