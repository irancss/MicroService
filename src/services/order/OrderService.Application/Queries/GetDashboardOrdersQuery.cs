using MediatR;
using OrderService.Application.DTOs;
using OrderService.Application.Services;
using AutoMapper;
using Microsoft.Extensions.Logging;
using OrderService.Core.Enums;

namespace OrderService.Application.Queries
{
    public class GetDashboardOrdersQuery : IRequest<PaginatedResult<OrderVM>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public OrderStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SortBy { get; set; } = "CreatedAt";
        public bool SortAscending { get; set; } = false;
    }

    public class GetDashboardOrdersQueryHandler : IRequestHandler<GetDashboardOrdersQuery, PaginatedResult<OrderVM>>
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetDashboardOrdersQueryHandler> _logger;

        public GetDashboardOrdersQueryHandler(
            IOrderService orderService,
            IMapper mapper,
            ILogger<GetDashboardOrdersQueryHandler> logger)
        {
            _orderService = orderService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<OrderVM>> Handle(GetDashboardOrdersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Retrieving dashboard orders, page {PageNumber}, size {PageSize}", 
                    request.PageNumber, request.PageSize);

                var orders = await _orderService.GetOrdersAsync(
                    request.PageNumber,
                    request.PageSize,
                    request.Status,
                    request.FromDate,
                    request.ToDate,
                    request.SortBy ?? "CreatedAt",
                    request.SortAscending);

                var totalCount = await _orderService.GetOrdersCountAsync(
                    request.Status, 
                    request.FromDate, 
                    request.ToDate);

                var orderVMs = _mapper.Map<List<OrderVM>>(orders);

                var result = new PaginatedResult<OrderVM>
                {
                    Items = orderVMs,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
                };

                _logger.LogInformation("Successfully retrieved {Count} dashboard orders", orderVMs.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard orders");
                throw;
            }
        }
    }
}
