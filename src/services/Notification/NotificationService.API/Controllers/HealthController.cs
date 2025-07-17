using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NotificationService.Domain.Interfaces;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;
    private readonly IEmailProvider _emailProvider;
    private readonly ISmsProvider _smsProvider;

    public HealthController(
        HealthCheckService healthCheckService,
        IEmailProvider emailProvider,
        ISmsProvider smsProvider)
    {
        _healthCheckService = healthCheckService;
        _emailProvider = emailProvider;
        _smsProvider = smsProvider;
    }

    [HttpGet]
    public async Task<ActionResult> GetHealth()
    {
        var report = await _healthCheckService.CheckHealthAsync();
        
        var response = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds,
                description = e.Value.Description,
                data = e.Value.Data
            }),
            providers = new
            {
                email = new
                {
                    provider = _emailProvider.GetType().Name,
                    healthy = _emailProvider.IsHealthy()
                },
                sms = new
                {
                    provider = _smsProvider.GetType().Name,
                    healthy = _smsProvider.IsHealthy()
                }
            }
        };

        return report.Status == HealthStatus.Healthy
            ? Ok(response)
            : StatusCode(503, response);
    }

    [HttpGet("ready")]
    public ActionResult GetReadiness()
    {
        // Simple readiness check
        return Ok(new { status = "Ready", timestamp = DateTime.UtcNow });
    }

    [HttpGet("live")]
    public ActionResult GetLiveness()
    {
        // Simple liveness check
        return Ok(new { status = "Alive", timestamp = DateTime.UtcNow });
    }
}
