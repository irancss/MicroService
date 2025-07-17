using AutoMapper;
using MediatR;
using TicketService.Application.DTOs;
using TicketService.Application.Interfaces;

namespace TicketService.Application.CQRS.Tag.Commands;

public class CreateTicketRequestCommand : IRequest<string>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int UserId { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }

    public CreateTicketRequestCommand(string title, string description, int userId, string status, string priority)
    {
        Title = title;
        Description = description;
        UserId = userId;
        Status = status;
        Priority = priority;
    }
}

public class CreateTicketRequestCommandHandler : IRequestHandler<CreateTicketRequestCommand, string>
{
    private readonly ITicketService _ticketService;
    private readonly IMapper _mapper;

    public CreateTicketRequestCommandHandler(ITicketService ticketService, IMapper mapper)
    {
        _ticketService = ticketService;
        _mapper = mapper;
    }

    public async Task<string> Handle(CreateTicketRequestCommand request, CancellationToken cancellationToken)
    {
        var map = _mapper.Map<CreateTicketRequestCommand, TicketDto>(request);
       return  await _ticketService.AddTicketAsync(map);
    }
}
