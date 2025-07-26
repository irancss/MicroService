using AutoMapper;
using MediatR;
using TicketService.Application.DTOs;
using TicketService.Application.Interfaces;

namespace TicketService.Application.CQRS.Seller.Commands
{
    public class AssignTicketToUserCommand : IRequest<IEnumerable<TicketDto>>
    {
        public string TicketId { get; set; }
        public string UserId { get; set; }

       
    }

    public class AssignTicketToUserCommandHandler : IRequestHandler<AssignTicketToUserCommand, IEnumerable<TicketDto>>
    {
        private readonly ITicketService _ticketService;
        private readonly IMapper _mapper;

        public AssignTicketToUserCommandHandler(ITicketService ticketService, IMapper mapper)
        {
            _ticketService = ticketService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TicketDto>> Handle(AssignTicketToUserCommand request, CancellationToken cancellationToken)
        {
            //var tickets = await _ticketService.AssignTicketToUserAsync(request.TicketId, request.UserId, cancellationToken);
            //return _mapper.Map<IEnumerable<TicketDto>>(tickets);
            return new List<TicketDto>();
        }
    }
}