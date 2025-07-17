using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.DTOs;
using Payment.Application.UseCases.Wallets;
using Payment.Application.Validators;
using System.Security.Claims;

namespace Payment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<WalletController> _logger;

    public WalletController(IMediator mediator, ILogger<WalletController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get user's wallet information
    /// </summary>
    /// <returns>Wallet details</returns>
    [HttpGet]
    public async Task<IActionResult> GetWallet()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            var query = new GetWalletQuery(userId);
            var wallet = await _mediator.Send(query);

            if (wallet == null)
            {
                return NotFound(new { message = "کیف پول یافت نشد" });
            }

            return Ok(wallet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting wallet for user");
            return StatusCode(500, new { message = "خطای سیستمی در دریافت اطلاعات کیف پول" });
        }
    }

    /// <summary>
    /// Get specific user's wallet (Admin only)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Wallet details</returns>
    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserWallet([FromRoute] Guid userId)
    {
        try
        {
            var query = new GetWalletQuery(userId);
            var wallet = await _mediator.Send(query);

            if (wallet == null)
            {
                return NotFound(new { message = "کیف پول کاربر یافت نشد" });
            }

            return Ok(wallet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting wallet for user {UserId}", userId);
            return StatusCode(500, new { message = "خطای سیستمی در دریافت اطلاعات کیف پول" });
        }
    }

    /// <summary>
    /// Deposit money to wallet
    /// </summary>
    /// <param name="depositDto">Deposit information</param>
    /// <returns>Updated wallet information</returns>
    [HttpPost("deposit")]
    public async Task<IActionResult> DepositToWallet([FromBody] WalletDepositDto depositDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            // Set the user ID from token
            depositDto.UserId = userId;

            var validator = new WalletDepositDtoValidator();
            var validationResult = await validator.ValidateAsync(depositDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(new { 
                    message = "داده‌های واریز نامعتبر است",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage)
                });
            }

            var command = new DepositToWalletCommand(depositDto);
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(result.Wallet);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error depositing to wallet for user");
            return StatusCode(500, new { message = "خطای سیستمی در واریز به کیف پول" });
        }
    }

    /// <summary>
    /// Withdraw money from wallet
    /// </summary>
    /// <param name="withdrawalDto">Withdrawal information</param>
    /// <returns>Updated wallet information</returns>
    [HttpPost("withdraw")]
    public async Task<IActionResult> WithdrawFromWallet([FromBody] WalletWithdrawalDto withdrawalDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            // Set the user ID from token
            withdrawalDto.UserId = userId;

            var validator = new WalletWithdrawalDtoValidator();
            var validationResult = await validator.ValidateAsync(withdrawalDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(new { 
                    message = "داده‌های برداشت نامعتبر است",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage)
                });
            }

            var command = new WithdrawFromWalletCommand(withdrawalDto);
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(result.Wallet);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error withdrawing from wallet for user");
            return StatusCode(500, new { message = "خطای سیستمی در برداشت از کیف پول" });
        }
    }

    /// <summary>
    /// Make a purchase using wallet credit
    /// </summary>
    /// <param name="purchaseDto">Purchase information</param>
    /// <returns>Updated wallet information</returns>
    [HttpPost("purchase")]
    public async Task<IActionResult> PurchaseWithWallet([FromBody] WalletPurchaseDto purchaseDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            // Set the user ID from token
            purchaseDto.UserId = userId;

            // Basic validation (you can add a validator for purchase DTO if needed)
            if (purchaseDto.Amount <= 0)
            {
                return BadRequest(new { message = "مبلغ خرید باید بزرگتر از صفر باشد" });
            }

            var command = new PurchaseWithWalletCommand(purchaseDto);
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(result.Wallet);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error making purchase with wallet for user");
            return StatusCode(500, new { message = "خطای سیستمی در خرید با کیف پول" });
        }
    }

    /// <summary>
    /// Admin deposit to any user's wallet
    /// </summary>
    /// <param name="depositDto">Deposit information with target user ID</param>
    /// <returns>Updated wallet information</returns>
    [HttpPost("admin/deposit")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminDepositToWallet([FromBody] WalletDepositDto depositDto)
    {
        try
        {
            var validator = new WalletDepositDtoValidator();
            var validationResult = await validator.ValidateAsync(depositDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(new { 
                    message = "داده‌های واریز نامعتبر است",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage)
                });
            }

            var command = new DepositToWalletCommand(depositDto);
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(result.Wallet);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in admin deposit to wallet for user {UserId}", depositDto.UserId);
            return StatusCode(500, new { message = "خطای سیستمی در واریز توسط ادمین" });
        }
    }
}
