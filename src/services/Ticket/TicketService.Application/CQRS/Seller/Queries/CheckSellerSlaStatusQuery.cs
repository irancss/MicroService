using AutoMapper;
using MediatR;
using TicketService.Application.DTOs;
using TicketService.Application.Interfaces;

namespace TicketService.Application.CQRS.Seller.Queries
{

    public class CheckSellerSlaStatusQuery : IRequest<IEnumerable<TicketDto>>
    {
        public string SellerId { get; set; }

        public CheckSellerSlaStatusQuery(string sellerId)
        {
            SellerId = sellerId;
        }
    }

    public class CheckSellerSlaStatusQueryHandler : IRequestHandler<CheckSellerSlaStatusQuery, IEnumerable<TicketDto>>
    {
        private readonly ITicketService _ticketService;
        private readonly IMapper _mapper;

        public CheckSellerSlaStatusQueryHandler(ITicketService ticketService, IMapper mapper)
        {
            _ticketService = ticketService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TicketDto>> Handle(CheckSellerSlaStatusQuery request, CancellationToken cancellationToken)
        {
            return _mapper.Map<IEnumerable<TicketDto>>(
                await _ticketService.CheckSellerSlaStatusAsync(request.SellerId, cancellationToken));
        }
    }
}