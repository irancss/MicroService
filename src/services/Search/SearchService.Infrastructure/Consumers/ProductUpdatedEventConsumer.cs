using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using SearchService.Application.Commands.IndexProduct;
using SearchService.Domain.Entities;
using SearchService.Infrastructure.Events;

namespace SearchService.Infrastructure.Consumers;

/// <summary>
/// Consumer for ProductUpdatedEvent to update indexed products
/// </summary>
public class ProductUpdatedEventConsumer : IConsumer<ProductUpdatedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductUpdatedEventConsumer> _logger;

    public ProductUpdatedEventConsumer(IMediator mediator, ILogger<ProductUpdatedEventConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductUpdatedEvent> context)
    {
        var productEvent = context.Message;

        _logger.LogInformation("Processing ProductUpdatedEvent for product: {ProductId}", productEvent.ProductId);

        try
        {
            var productDocument = MapToProductDocument(productEvent);

            var command = new IndexProductCommand
            {
                Product = productDocument
            };

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                _logger.LogInformation("Successfully updated product index: {ProductId}", productEvent.ProductId);
            }
            else
            {
                _logger.LogError("Failed to update product index: {ProductId}, Error: {Error}", 
                    productEvent.ProductId, result.ErrorMessage);
                throw new InvalidOperationException($"Failed to update product index: {result.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing ProductUpdatedEvent for product: {ProductId}", productEvent.ProductId);
            throw;
        }
    }

    private ProductDocument MapToProductDocument(ProductUpdatedEvent productEvent)
    {
        return new ProductDocument
        {
            Id = productEvent.ProductId,
            Name = productEvent.Name,
            Description = productEvent.Description,
            Category = productEvent.Category,
            Brand = productEvent.Brand,
            Price = productEvent.Price,
            AverageRating = productEvent.AverageRating,
            ReviewCount = productEvent.ReviewCount,
            IsAvailable = productEvent.IsAvailable,
            StockQuantity = productEvent.StockQuantity,
            UpdatedAt = productEvent.UpdatedAt,
            Tags = productEvent.Tags,
            ImageUrls = productEvent.ImageUrls,
            Attributes = productEvent.Attributes,
            Slug = productEvent.Slug,
            Weight = productEvent.Weight
        };
    }
}
