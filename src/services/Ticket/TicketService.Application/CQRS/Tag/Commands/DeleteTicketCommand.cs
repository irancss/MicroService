using MediatR;
using TicketService.Application.Interfaces;

namespace TicketService.Application.CQRS.Tag.Commands;

public class DeleteTicketCommand : IRequest<bool>
{
    public string TicketId { get; set; }

    public DeleteTicketCommand(string ticketId)
    {
        TicketId = ticketId;
    }
}

public class DeleteTicketCommandHandler : IRequestHandler<DeleteTicketCommand, bool>
{
    private readonly ITicketService _ticketService;

    public DeleteTicketCommandHandler(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    public async Task<bool> Handle(DeleteTicketCommand request, CancellationToken cancellationToken)
    {
        return await _ticketService.DeleteTicketAsync(request.TicketId, cancellationToken);
    }
}
