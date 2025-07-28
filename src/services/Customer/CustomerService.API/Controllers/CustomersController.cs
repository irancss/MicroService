using BuildingBlocks.Common;
using CustomerService.Application.Commands.CreateCustomer;
using CustomerService.Application.Dtos;
using CustomerService.Application.Queries.GetCustomerById;
using CustomerService.Application.Queries.GetCustomers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // تمام endpoint ها نیاز به احراز هویت دارند
    public class CustomersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            var query = new GetCustomerByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IPaginate<CustomerDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCustomers([FromQuery] GetCustomersQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerCommand command)
        {
            var customerId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetCustomerById), new { id = customerId }, customerId);
        }
    }
}