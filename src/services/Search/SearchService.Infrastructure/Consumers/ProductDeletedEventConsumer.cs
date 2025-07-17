using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using SearchService.Application.Commands.RemoveProduct;
using SearchService.Infrastructure.Events;

namespace SearchService.Infrastructure.Consumers;

/// <summary>
/// Consumer for ProductDeletedEvent to remove products from index
/// </summary>
public class ProductDeletedEventConsumer : IConsumer<ProductDeletedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductDeletedEventConsumer> _logger;

    public ProductDeletedEventConsumer(IMediator mediator, ILogger<ProductDeletedEventConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductDeletedEvent> context)
    {
        var productEvent = context.Message;

        _logger.LogInformation("Processing ProductDeletedEvent for product: {ProductId}", productEvent.ProductId);

        try
        {
            var command = new RemoveProductFromIndexCommand
            {
                ProductId = productEvent.ProductId
            };

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                _logger.LogInformation("Successfully removed product from index: {ProductId}", productEvent.ProductId);
            }
            else
            {
                _logger.LogError("Failed to remove product from index: {ProductId}, Error: {Error}", 
                    productEvent.ProductId, result.ErrorMessage);
                throw new InvalidOperationException($"Failed to remove product from index: {result.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing ProductDeletedEvent for product: {ProductId}", productEvent.ProductId);
            throw;
        }
    }
}
