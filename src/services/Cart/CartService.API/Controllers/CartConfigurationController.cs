using Cart.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cart.API.Controllers;

[ApiController]
[Route("api/v1/admin/cart-config")]
[Authorize(Roles = "admin")] // Only accessible by users with the 'admin' role
public class CartConfigurationController : ControllerBase
{
    private readonly ICartConfigurationService _configService;
    private readonly ILogger<CartConfigurationController> _logger;

    public CartConfigurationController(ICartConfigurationService configService, ILogger<CartConfigurationController> logger)
    {
        _configService = configService;
        _logger = logger;
    }

    /// <summary>
    /// Gets the current global cart configuration.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CartConfiguration), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartConfiguration>> GetConfiguration()
    {
        var config = await _configService.GetConfigurationAsync();
        return Ok(config);
    }

    /// <summary>
    /// Updates the global cart configuration.
    /// </summary>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateConfiguration([FromBody] CartConfiguration configuration)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _configService.UpdateConfigurationAsync(configuration);
        _logger.LogInformation("Cart configuration updated by an administrator.");
        return NoContent();
    }
}