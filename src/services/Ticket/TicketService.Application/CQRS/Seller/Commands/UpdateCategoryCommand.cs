using AutoMapper;

namespace TicketService.Application.CQRS.Seller.Commands
{
    public class UpdateCategoryCommand : IRequest<bool>
    {
        public string SellerId { get; set; }
        public string CategoryName { get; set; }

        public UpdateCategoryCommand(string sellerId, string categoryName)
        {
            SellerId = sellerId;
            CategoryName = categoryName;
        }
    }

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, bool>
    {
        private readonly ITicketService _ticketService;
       private readonly IMapper _mapper;

        public UpdateCategoryCommandHandler(ITicketService ticketService, IMapper mapper)
        {
            _ticketService = ticketService;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            return await _ticketService.UpdateCategoryAsync(request.SellerId, cancellationToken);
        }
    }
}