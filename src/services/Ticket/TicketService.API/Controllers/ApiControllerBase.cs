using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace TicketService.API.Controllers;

[Route("api/[controller]")] // Changed from [Microsoft.AspNetCore.Components.Route("api/[controller]/[action]")]
[ApiController]
[EnableCors("AllowspecificOrigins")]
public abstract class ApiControllerBase : ControllerBase
{
  private ISender _mediator = null!;
  protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}

