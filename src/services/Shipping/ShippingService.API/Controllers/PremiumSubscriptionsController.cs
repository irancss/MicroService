using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShippingService.Application.Features.PremiumSubscriptions.Commands;
using ShippingService.Application.Features.PremiumSubscriptions.Queries;

namespace ShippingService.API.Controllers;

/// <summary>
/// کنترلر مدیریت اشتراک‌های ویژه
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PremiumSubscriptionsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// سازنده کنترلر اشتراک‌های ویژه
    /// </summary>
    /// <param name="mediator">واسط MediatR</param>
    public PremiumSubscriptionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// دریافت اشتراک فعال کاربر
    /// </summary>
    /// <param name="userId">شناسه کاربر</param>
    /// <returns>اطلاعات اشتراک فعال</returns>
    [HttpGet("user/{userId}/active")]
    public async Task<ActionResult> GetActiveSubscription(string userId)
    {
        var result = await _mediator.Send(new GetActiveSubscriptionQuery(userId));
        
        if (result == null)
        {
            return NotFound(new { message = "اشتراک فعالی یافت نشد" });
        }

        return Ok(result);
    }

    /// <summary>
    /// ایجاد اشتراک جدید
    /// </summary>
    /// <param name="command">اطلاعات اشتراک جدید</param>
    /// <returns>اطلاعات اشتراک ایجاد شده</returns>
    [HttpPost]
    public async Task<ActionResult> CreateSubscription([FromBody] CreateSubscriptionCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetActiveSubscription), 
            new { userId = result.UserId }, result);
    }

    /// <summary>
    /// تمدید اشتراک
    /// </summary>
    /// <param name="subscriptionId">شناسه اشتراک</param>
    /// <param name="command">اطلاعات تمدید</param>
    /// <returns>نتیجه تمدید</returns>
    [HttpPut("{subscriptionId}/extend")]
    public async Task<ActionResult> ExtendSubscription(
        Guid subscriptionId, 
        [FromBody] ExtendSubscriptionCommand command)
    {
        command.SubscriptionId = subscriptionId;
        var result = await _mediator.Send(command);
        
        if (!result)
        {
            return BadRequest(new { message = "تمدید اشتراک انجام نشد" });
        }

        return Ok(new { message = "اشتراک با موفقیت تمدید شد" });
    }

    /// <summary>
    /// لغو اشتراک
    /// </summary>
    /// <param name="subscriptionId">شناسه اشتراک</param>
    /// <returns>نتیجه لغو</returns>
    [HttpPut("{subscriptionId}/cancel")]
    public async Task<ActionResult> CancelSubscription(Guid subscriptionId)
    {
        var result = await _mediator.Send(new CancelSubscriptionCommand(subscriptionId));
        
        if (!result)
        {
            return BadRequest(new { message = "لغو اشتراک انجام نشد" });
        }

        return Ok(new { message = "اشتراک با موفقیت لغو شد" });
    }

    /// <summary>
    /// استفاده از درخواست رایگان
    /// </summary>
    /// <param name="command">اطلاعات استفاده</param>
    /// <returns>نتیجه استفاده</returns>
    [HttpPost("use-free-request")]
    public async Task<ActionResult> UseFreeRequest([FromBody] UseFreeRequestCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result)
        {
            return BadRequest(new { message = "استفاده از درخواست رایگان انجام نشد" });
        }

        return Ok(new { message = "درخواست رایگان استفاده شد" });
    }

    /// <summary>
    /// بررسی امکان استفاده از درخواست رایگان
    /// </summary>
    /// <param name="userId">شناسه کاربر</param>
    /// <returns>آیا می‌توان استفاده کرد؟</returns>
    [HttpGet("user/{userId}/can-use-free-request")]
    public async Task<ActionResult> CanUseFreeRequest(string userId)
    {
        var result = await _mediator.Send(new CanUseFreeRequestQuery(userId));
        return Ok(new { canUse = result });
    }

    /// <summary>
    /// دریافت تاریخچه استفاده از اشتراک
    /// </summary>
    /// <param name="subscriptionId">شناسه اشتراک</param>
    /// <returns>تاریخچه استفاده</returns>
    [HttpGet("{subscriptionId}/usage-history")]
    public async Task<ActionResult> GetUsageHistory(Guid subscriptionId)
    {
        var result = await _mediator.Send(new GetSubscriptionUsageHistoryQuery(subscriptionId));
        return Ok(result);
    }
}
