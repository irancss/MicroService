using Microsoft.AspNetCore.Mvc;
using TicketService.Application.CQRS.Seller.Commands;
using TicketService.Application.CQRS.Seller.Queries;
using TicketService.Application.DTOs;

namespace TicketService.API.Controllers.Ticket
{
    public class SellerController : ApiControllerBase // Assuming ApiControllerBase is similar or you can adjust
    {
        private readonly ILogger<SellerController> _logger;

        public SellerController(ILogger<SellerController> logger)
        {
            _logger = logger;
        }

        // 1. پشتیبانی از فروشندگان (Seller-Specific Tickets)
        // GET /api/Ticket/seller/{sellerId}
        // دریافت تیکت‌های مربوط به یک فروشنده خاص.
        // این API به فروشندگان اجازه می‌دهد تا فقط تیکت‌های مرتبط با خودشان را مشاهده کنند.
        // 1. دریافت تیکت‌های یک فروشنده خاص با قابلیت صفحه‌بندی
        [HttpGet("{sellerId}")]
        public async Task<IActionResult> GetTicketsBySeller(string sellerId, int page = 1, int pageSize = 10, string sortBy = "date", string filter = null)
        {
            _logger.LogInformation("Fetching tickets for sellerId: {SellerId}, page: {Page}, pageSize: {PageSize}, sortBy: {SortBy}, filter: {Filter}", sellerId, page, pageSize, sortBy, filter);
            var tickets = await Mediator.Send(new GetTicketsBySellerQuery(sellerId, page, pageSize, sortBy, filter));
            if (tickets == null || !tickets.Any())
            {
                _logger.LogWarning("No tickets found for sellerId: {SellerId}", sellerId);
                return NotFound(new { message = $"No tickets found for seller {sellerId}" });
            }
            return Ok(tickets);
        }

        // 2. ایجاد تیکت برای یک فروشنده خاص
        [HttpPost("{sellerId}")]
        public async Task<IActionResult> CreateTicketForSeller(string sellerId, [FromBody] TicketDto ticketData)
        {
            _logger.LogInformation("Creating a new ticket for sellerId: {SellerId}", sellerId);
            var command = new CreateTicketForSellerCommand
            {
                SellerId = sellerId,
                TicketDetails = ticketData
            };
            var newTicketId = await Mediator.Send(command);
            if (newTicketId == null)
            {
                _logger.LogError("Failed to create ticket for sellerId: {SellerId}", sellerId);
                return BadRequest(new { message = "Failed to create ticket" });
            }
            return Created($"api/Ticket/seller/{sellerId}/{newTicketId}", new { message = $"Ticket created for seller {sellerId}", data = newTicketId });
        }

        // ... rest of the file ...

        [HttpGet("{sellerId}/report")]
        public async Task<IActionResult> GetSellerReport(string sellerId)
        {
            _logger.LogInformation("Generating report for sellerId: {SellerId}", sellerId);
            var report = await Mediator.Send(new GetSellerReportQuery { sellerId = sellerId });
            if (report == null)
            {
                _logger.LogWarning("No report found for sellerId: {SellerId}", sellerId);
                return NotFound(new { message = $"No report found for seller {sellerId}" });
            }
            return Ok(report);
        }

        // 4. بررسی وضعیت SLA برای تیکت‌های یک فروشنده خاص
        [HttpGet("{sellerId}/sla")]
        public async Task<IActionResult> CheckSellerSlaStatus(string sellerId)
        {
            _logger.LogInformation("Checking SLA status for sellerId: {SellerId}", sellerId);
            var status = await Mediator.Send(new CheckSellerSlaStatusQuery(sellerId));
            return Ok(status);
        }

        // 5. اختصاص یک تیکت به یک کاربر خاص (برای فروشنده)
        [HttpPut("{ticketId}/assign/{userId}")]
        public async Task<IActionResult> AssignTicketToUser(string ticketId, string userId)
        {
            _logger.LogInformation("Assigning ticketId: {TicketId} to userId: {UserId}", ticketId, userId);
            var command = new AssignTicketToUserCommand
            {
                TicketId = ticketId,
                UserId = userId
            };
            var result = await Mediator.Send(command);
            if (result.Count() > 0)
            {
                _logger.LogWarning("Failed to assign ticketId: {TicketId} to userId: {UserId}", ticketId, userId);
                return BadRequest(new { message = $"Failed to assign ticket {ticketId} to user {userId}" });
            }
            return Ok(new { message = $"Ticket {ticketId} assigned to user {userId}" });
        }
    }
}
