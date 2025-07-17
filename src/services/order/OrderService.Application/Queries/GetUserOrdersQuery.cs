using MediatR;
using OrderService.Application.DTOs;
using OrderService.Application.Services;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace OrderService.Application.Queries
{
    public class GetUserOrdersQuery : IRequest<PaginatedResult<OrderVM>>
    {
        public Guid UserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "CreatedAt";
        public bool SortAscending { get; set; } = false;
    }

    public class GetUserOrdersQueryHandler : IRequestHandler<GetUserOrdersQuery, PaginatedResult<OrderVM>>
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserOrdersQueryHandler> _logger;

        public GetUserOrdersQueryHandler(
            IOrderService orderService,
            IMapper mapper,
            ILogger<GetUserOrdersQueryHandler> logger)
        {
            _orderService = orderService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<OrderVM>> Handle(GetUserOrdersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Retrieving orders for user {UserId}, page {PageNumber}, size {PageSize}", 
                    request.UserId, request.PageNumber, request.PageSize);

                var orders = await _orderService.GetOrdersByUserAsync(
                    request.UserId, 
                    request.PageNumber, 
                    request.PageSize, 
                    request.SortBy ?? "CreatedAt", 
                    request.SortAscending);

                var totalCount = await _orderService.GetUserOrdersCountAsync(request.UserId);

                var orderVMs = _mapper.Map<List<OrderVM>>(orders);

                var result = new PaginatedResult<OrderVM>
                {
                    Items = orderVMs,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
                };

                _logger.LogInformation("Successfully retrieved {Count} orders for user {UserId}", 
                    orderVMs.Count, request.UserId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders for user {UserId}", request.UserId);
                throw;
            }
        }
    }

    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
