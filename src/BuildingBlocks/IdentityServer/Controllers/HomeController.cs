using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers;

[Route("")]
public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { 
            status = "healthy", 
            service = "IdentityServer8", 
            timestamp = DateTime.UtcNow,
            version = "8.0.4"
        });
    }

    [HttpGet("error")]
    public IActionResult Error()
    {
        return View();
    }
}
