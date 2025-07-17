namespace TicketService.Application.CQRS.Seller.Queries
{

    public class GetSellerReportQuery : IRequest<IEnumerable<TicketDto>>
    {
        public string sellerId { get; set; }
    }

    public class GetSellerReportQueryHandler : IRequestHandler<GetSellerReportQuery, IEnumerable<TicketDto>>
    {
        private readonly ITicketService _ticketService;
        private readonly IMapper _mapper;

        public GetSellerReportQueryHandler(ITicketService ticketService, IMapper mapper)
        {
            _ticketService = ticketService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TicketDto>> Handle(GetSellerReportQuery request, CancellationToken cancellationToken)
        {
            return _mapper.Map<IEnumerable<TicketDto>>(
                await _ticketService.GetTicketsBySellerAsync(request.SellerId, cancellationToken));
        }
    }
}