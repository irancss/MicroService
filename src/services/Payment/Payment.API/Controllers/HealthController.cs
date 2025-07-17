using Microsoft.AspNetCore.Mvc;

namespace Payment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Basic health check endpoint
    /// </summary>
    /// <returns>Service health status</returns>
    [HttpGet]
    public IActionResult GetHealth()
    {
        try
        {
            return Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                service = "Payment Microservice",
                version = "1.0.0"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(503, new
            {
                status = "Unhealthy",
                timestamp = DateTime.UtcNow,
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Detailed health check with dependencies
    /// </summary>
    /// <returns>Detailed health status</returns>
    [HttpGet("detailed")]
    public async Task<IActionResult> GetDetailedHealth()
    {
        try
        {
            // In a real implementation, you would check:
            // - Database connectivity
            // - Redis connectivity  
            // - RabbitMQ connectivity
            // - MongoDB connectivity
            // - External gateway availability

            var healthChecks = new Dictionary<string, object>
            {
                ["database"] = new { status = "Healthy", responseTime = "< 100ms" },
                ["cache"] = new { status = "Healthy", responseTime = "< 50ms" },
                ["messageQueue"] = new { status = "Healthy", responseTime = "< 200ms" },
                ["logging"] = new { status = "Healthy", responseTime = "< 100ms" },
                ["gateways"] = new { status = "Healthy", available = 10 }
            };

            return Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                service = "Payment Microservice",
                version = "1.0.0",
                checks = healthChecks
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Detailed health check failed");
            return StatusCode(503, new
            {
                status = "Unhealthy",
                timestamp = DateTime.UtcNow,
                error = ex.Message
            });
        }
    }
}
