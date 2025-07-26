using AutoMapper;
using MediatR;
using TicketService.Application.DTOs;
using TicketService.Application.Interfaces;

public class GetTicketsBySellerQuery : IRequest<IEnumerable<TicketDto>>
{
    public string SellerId { get; set; }
    public int Page { get; set; } = 1; // Default to page 1
    public int PageSize { get; set; } = 10; // Default to 10 items per page
    public string SortBy { get; set; } = "date"; // Default sorting by date
    public string Filter { get; set; } = null; // Default to no filter
}

public class GetTicketsBySellerQueryHandler : IRequestHandler<GetTicketsBySellerQuery, IEnumerable<TicketDto>>
{
    private readonly ITicketService _ticketService;
    private readonly IMapper _mapper;

    public GetTicketsBySellerQueryHandler(ITicketService ticketService, IMapper mapper)
    {
        _ticketService = ticketService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TicketDto>> Handle(GetTicketsBySellerQuery request, CancellationToken cancellationToken)
    {
        //return _mapper.Map<IEnumerable<TicketDto>>(
          //  await _ticketService.GetTicketsBySellerAsync(request.SellerId, request.status, cancellationToken));
          return new List<TicketDto>();
    }
}