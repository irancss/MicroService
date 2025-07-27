using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.CQRS.Brand;
using System.Net;
using ProductService.API.Controllers;
using ProductService.Application.DTOs.Brand;

[Route("api/brands")] // مسیر استاندارد RESTful
public class BrandsController : ApiControllerBase
{
    // DTO برای ایجاد برند
    public record CreateBrandRequest(string Name, string? Description);

    [HttpPost]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateBrand([FromBody] CreateBrandRequest request)
    {
        var command = new CreateBrandCommand(request.Name, request.Description);
        var brandId = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetBrandById), new { brandId }, brandId);
    }

    [HttpGet("{brandId:guid}")]
    [ProducesResponseType(typeof(BrandDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetBrandById(Guid brandId)
    {
        var brand = await Mediator.Send(new GetBrandByIdQuery(brandId));
        return brand is not null ? Ok(brand) : NotFound();
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BrandDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAllBrands()
    {
        var brands = await Mediator.Send(new GetAllBrandsQuery());
        return Ok(brands);
    }

    // DTO برای به‌روزرسانی برند
    public record UpdateBrandRequest(string Name, string? Description);

    [HttpPut("{brandId:guid}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> UpdateBrand(Guid brandId, [FromBody] UpdateBrandRequest request)
    {
        var command = new UpdateBrandCommand(brandId, request.Name, request.Description);
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{brandId:guid}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> DeleteBrand(Guid brandId)
    {
        await Mediator.Send(new DeleteBrandCommand(brandId));
        return NoContent();
    }
}