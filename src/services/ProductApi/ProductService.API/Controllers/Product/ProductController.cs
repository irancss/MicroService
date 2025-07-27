using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.CQRS.Commands;
using ProductService.Application.DTOs.Product;
using System.Net;
using ProductService.API.Controllers;
using ProductService.Application.CQRS.Queries;

public class ProductsController : ApiControllerBase // ارث‌بری از کنترلر پایه
{
    [HttpPost]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        var productId = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetProductById), new { productId }, productId);
    }

    [HttpGet("{productId:guid}")]
    [ProducesResponseType(typeof(ProductDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetProductById(Guid productId)
    {
        var product = await Mediator.Send(new GetProductByIdQuery(productId));
        return product is not null ? Ok(product) : NotFound();
    }

    [HttpPut("{productId:guid}/stock")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateStock(Guid productId, [FromBody] UpdateProductStockRequest request)
    {
        var command = new UpdateProductStockCommand(productId, request.NewQuantity);
        await Mediator.Send(command);
        return NoContent();
    }

    // یک DTO ساده برای درخواست آپدیت موجودی
    public record UpdateProductStockRequest(int NewQuantity);
}