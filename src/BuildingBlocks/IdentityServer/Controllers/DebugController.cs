using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers;

[ApiController]
[Route("debug")]
public class DebugController : ControllerBase
{
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new { 
            message = "Debug endpoint working",
            timestamp = DateTime.UtcNow,
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
        });
    }

    [HttpGet("routes")]
    public IActionResult GetRoutes()
    {
        var routes = HttpContext.GetRouteData();
        return Ok(new { 
            routeData = routes.Values,
            request = new {
                path = HttpContext.Request.Path,
                method = HttpContext.Request.Method,
                scheme = HttpContext.Request.Scheme,
                host = HttpContext.Request.Host.Value
            }
        });
    }

    [HttpGet("identityserver-test")]
    public IActionResult TestIdentityServer()
    {
        try
        {
            var discoveryUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/.well-known/openid_configuration";
            return Ok(new { 
                message = "IdentityServer test endpoint",
                discoveryUrl = discoveryUrl,
                scheme = HttpContext.Request.Scheme,
                host = HttpContext.Request.Host.Value,
                path = HttpContext.Request.Path,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            });
        }
        catch (Exception ex)
        {
            return Ok(new { error = ex.Message });
        }
    }
}
