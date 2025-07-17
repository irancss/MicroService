using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.DTOs;
using Payment.Application.UseCases.Payments;
using Payment.Application.Validators;
using Payment.Domain.Interfaces;
using System.Security.Claims;

namespace Payment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IPaymentGatewayFactory _gatewayFactory;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        IMediator mediator,
        IPaymentGatewayFactory gatewayFactory,
        ILogger<PaymentController> logger)
    {
        _mediator = mediator;
        _gatewayFactory = gatewayFactory;
        _logger = logger;
    }

    /// <summary>
    /// Get available payment gateways
    /// </summary>
    /// <returns>List of available gateways</returns>
    [HttpGet("gateways")]
    public IActionResult GetAvailableGateways()
    {
        try
        {
            // Check if user is admin to include test gateways
            var isAdmin = User.IsInRole("Admin");
            
            var gateways = _gatewayFactory.GetAvailableGateways(isAdmin)
                .Select(g => new GatewayInfoDto
                {
                    Name = g.GatewayName,
                    IsTestMode = g.IsTestMode,
                    IsAvailable = true
                });

            return Ok(gateways);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available gateways");
            return StatusCode(500, new { message = "خطا در دریافت لیست درگاه‌ها" });
        }
    }

    /// <summary>
    /// Initiate a new payment
    /// </summary>
    /// <param name="model">Payment initiation data</param>
    /// <returns>Payment URL for redirect</returns>
    [HttpPost("initiate")]
    public async Task<IActionResult> InitiatePayment([FromBody] PaymentInitiationDto model)
    {
        try
        {
            // Get user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            // Validate input
            var validator = new CreateTransactionDtoValidator();
            var transactionDto = new CreateTransactionDto
            {
                OrderId = model.OrderId,
                UserId = userId,
                Amount = model.Amount,
                Currency = "IRR", // Default to Iranian Rial
                GatewayName = model.GatewayName,
                Description = model.Description,
                CallbackUrl = $"{Request.Scheme}://{Request.Host}/api/payment/verify/{model.GatewayName}",
                Mobile = model.Mobile,
                Email = model.Email
            };

            var validationResult = await validator.ValidateAsync(transactionDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { 
                    message = "داده‌های ورودی نامعتبر است",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage)
                });
            }

            // Execute use case
            var command = new InitiatePaymentCommand(transactionDto);
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new { 
                    paymentUrl = result.PaymentUrl,
                    transactionId = result.TransactionId
                });
            }

            return BadRequest(new { message = result.ErrorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating payment for Order {OrderId}", model.OrderId);
            return StatusCode(500, new { message = "خطای سیستمی در ایجاد پرداخت" });
        }
    }

    /// <summary>
    /// Verify payment (callback from gateway)
    /// </summary>
    /// <param name="gatewayName">Name of the payment gateway</param>
    /// <returns>Verification result</returns>
    [HttpGet("verify/{gatewayName}")]
    [HttpPost("verify/{gatewayName}")]
    [AllowAnonymous] // Gateway callbacks don't have JWT tokens
    public async Task<IActionResult> VerifyPayment([FromRoute] string gatewayName)
    {
        try
        {
            // Extract gateway-specific data from query string or form
            var gatewayData = new Dictionary<string, string>();
            
            if (Request.Method == "GET")
            {
                foreach (var key in Request.Query.Keys)
                {
                    gatewayData[key] = Request.Query[key].ToString();
                }
            }
            else if (Request.Method == "POST")
            {
                foreach (var key in Request.Form.Keys)
                {
                    gatewayData[key] = Request.Form[key].ToString();
                }
            }

            // Extract OrderId from gateway data (implementation depends on gateway)
            var orderId = gatewayData.GetValueOrDefault("OrderId") ?? 
                         gatewayData.GetValueOrDefault("order_id") ??
                         gatewayData.GetValueOrDefault("ResNum");

            if (string.IsNullOrEmpty(orderId))
            {
                _logger.LogWarning("OrderId not found in gateway callback data for {GatewayName}", gatewayName);
                return Redirect($"/payment/failed?error=شماره_سفارش_یافت_نشد");
            }

            var verificationDto = new PaymentVerificationDto
            {
                GatewayName = gatewayName,
                OrderId = orderId,
                GatewayData = gatewayData
            };

            var command = new VerifyPaymentCommand(verificationDto);
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                // Redirect to success page
                return Redirect($"/payment/success?refId={result.ReferenceId}&orderId={orderId}");
            }

            // Redirect to failure page
            return Redirect($"/payment/failed?error={Uri.EscapeDataString(result.ErrorMessage ?? "خطای نامشخص")}&orderId={orderId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying payment for gateway {GatewayName}", gatewayName);
            return Redirect($"/payment/failed?error=خطای_سیستمی");
        }
    }

    /// <summary>
    /// Get transaction by ID
    /// </summary>
    /// <param name="transactionId">Transaction ID</param>
    /// <returns>Transaction details</returns>
    [HttpGet("transaction/{transactionId}")]
    public async Task<IActionResult> GetTransaction([FromRoute] Guid transactionId)
    {
        try
        {
            // Implementation would require a GetTransactionQuery use case
            // For now, return not implemented
            return StatusCode(501, new { message = "این قابلیت هنوز پیاده‌سازی نشده است" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction {TransactionId}", transactionId);
            return StatusCode(500, new { message = "خطای سیستمی" });
        }
    }

    /// <summary>
    /// Request refund for a transaction
    /// </summary>
    /// <param name="refundRequest">Refund request data</param>
    /// <returns>Refund result</returns>
    [HttpPost("refund")]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<IActionResult> RefundTransaction([FromBody] RefundRequestDto refundRequest)
    {
        try
        {
            var validator = new RefundRequestDtoValidator();
            var validationResult = await validator.ValidateAsync(refundRequest);
            
            if (!validationResult.IsValid)
            {
                return BadRequest(new { 
                    message = "داده‌های درخواست بازگشت وجه نامعتبر است",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage)
                });
            }

            // Implementation would require a RefundTransactionCommand use case
            // For now, return not implemented
            return StatusCode(501, new { message = "قابلیت بازگشت وجه هنوز پیاده‌سازی نشده است" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing refund for transaction {TransactionId}", refundRequest.TransactionId);
            return StatusCode(500, new { message = "خطای سیستمی در پردازش بازگشت وجه" });
        }
    }
}
