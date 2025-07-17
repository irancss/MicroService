using Microsoft.AspNetCore.Mvc;
using VendorService.Application.CQRS.Vendors.Commands;
using VendorService.Application.CQRS.Vendors.Queries;

namespace VendorService.API.Controllers.Vendor
{
    // This assumes you have a custom ApiControllerBase, otherwise inherit from ControllerBase
    public class VendorController : ApiControllerBase
    {
        private readonly ILogger<VendorController> _logger;

        public VendorController(ILogger<VendorController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetVendorNames()
        {
            // The following line assumes you have a Mediator instance available in your ApiControllerBase
            var vendorNames = await Mediator.Send(new GetVendorNamesQuery());
            return Ok(vendorNames);
        }

        [HttpGet]
        [Route("{id:string}")]
        [ProducesResponseType(typeof(VendorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetVendorById(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var vendorId))
            {
                return BadRequest("Invalid vendor ID.");
            }

            var vendor = await Mediator.Send(new GetVendorByIdQuery(id));
            if (vendor == null)
            {
                return NotFound($"Vendor with ID {id} not found.");
            }

            return Ok(vendor);
        }

        [HttpPost]
        [ProducesResponseType(typeof(VendorDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateVendor([FromBody] CreateVendorCommand command)
        {
            if (command == null)
            {
                return BadRequest("Invalid vendor data.");
            }

            var vendor = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetVendorById), new { id = vendor }, vendor);
        }

        [HttpPut]
        [Route("{id:string}")]
        [ProducesResponseType(typeof(VendorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateVendor(string id, [FromBody] UpdateVendorCommand command)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var vendorId))
            {
                return BadRequest("Invalid vendor ID.");
            }

            if (command == null)
            {
                return BadRequest("Invalid vendor data.");
            }

            command.VendorId = vendorId.ToString();

            var updatedVendor = await Mediator.Send(command);
            if (updatedVendor == null)
            {
                return NotFound($"Vendor with ID {id} not found.");
            }

            return Ok(updatedVendor);
        }

        [HttpDelete]
        [Route("{id:string}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteVendor(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var vendorId))
            {
                return BadRequest("Invalid vendor ID.");
            }

            var result = await Mediator.Send(new DeleteVendorCommand(vendorId.ToString()));
            if (!result)
            {
                return NotFound($"Vendor with ID {id} not found.");
            }

            return NoContent();
        }

        [HttpPost]
        [Route("{id:string}/changestatus")]
        [ProducesResponseType(typeof(VendorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeVendorStatus(string id, [FromBody] ChangeVendorStatusCommand command)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var vendorId))
            {
                return BadRequest("Invalid vendor ID.");
            }

            if (command == null)
            {
                return BadRequest("Invalid status data.");
            }

            command.VendorId = vendorId.ToString();

            var updatedVendor = await Mediator.Send(command);
            if (updatedVendor == null)
            {
                return NotFound($"Vendor with ID {id} not found.");
            }

            return Ok(updatedVendor);
        }

    

        [HttpGet]
        [Route("search")]
        [ProducesResponseType(typeof(IEnumerable<VendorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchVendors([FromQuery] string searchTerm , int pageNumber , int pageSize)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest("Search term cannot be empty.");
            }

            var vendors = await Mediator.Send(new SearchVendorsQuery(searchTerm , pageNumber, pageSize));
            if (vendors == null || !vendors.Any())
            {
                return NotFound($"No vendors found matching the search term '{searchTerm}'.");
            }

            return Ok(vendors);
        }

        [HttpGet]
        [Route("recent")]
        [ProducesResponseType(typeof(IEnumerable<VendorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRecentVendors(int count)
        {
            var vendors = await Mediator.Send(new GetRecentVendorsQuery(count));
            if (vendors == null || !vendors.Any())
            {
                return NotFound("No recent vendors found.");
            }

            return Ok(vendors);
        }

        [HttpGet]
        [Route("top-rated")]
        [ProducesResponseType(typeof(IEnumerable<VendorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTopRatedVendors(int count)
        {
            var vendors = await Mediator.Send(new GetTopRatedVendorsQuery(count));
            if (vendors == null || !vendors.Any())
            {
                return NotFound("No top-rated vendors found.");
            }

            return Ok(vendors);
        }

        [HttpGet]
        [Route("count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetVendorCount()
        {
            var count = await Mediator.Send(new GetVendorCountQuery());
            return Ok(count);
        }

        
    }
}

