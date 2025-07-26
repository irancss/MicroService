using TicketService.Application.DTOs;

namespace TicketService.Application.CQRS.Seller.Commands
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using TicketService.Application.Interfaces;

    public class CreateTicketForSellerCommand : IRequest<string>
    {
        public string SellerId { get; set; }
        public TicketDto TicketDetails { get; set; }
    }

    public class CreateTicketForSellerCommandHandler : IRequestHandler<CreateTicketForSellerCommand, string>
    {
        private readonly ITicketService _ticketService;

        public CreateTicketForSellerCommandHandler(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public async Task<string> Handle(CreateTicketForSellerCommand request, CancellationToken cancellationToken)
        {
            //return await _ticketService.CreateTicketForSellerAsync(request.SellerId, request.TicketDetails, cancellationToken);
            return "";
        }
    }
}