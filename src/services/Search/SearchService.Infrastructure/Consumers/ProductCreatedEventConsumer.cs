using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using SearchService.Application.Commands.IndexProduct;
using SearchService.Domain.Entities;
using SearchService.Infrastructure.Events;

namespace SearchService.Infrastructure.Consumers;

/// <summary>
/// Consumer for ProductCreatedEvent to index new products
/// </summary>
public class ProductCreatedEventConsumer : IConsumer<ProductCreatedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductCreatedEventConsumer> _logger;

    public ProductCreatedEventConsumer(IMediator mediator, ILogger<ProductCreatedEventConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        var productEvent = context.Message;

        _logger.LogInformation("Processing ProductCreatedEvent for product: {ProductId}", productEvent.ProductId);

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
                _logger.LogInformation("Successfully indexed product: {ProductId}", productEvent.ProductId);
            }
            else
            {
                _logger.LogError("Failed to index product: {ProductId}, Error: {Error}", 
                    productEvent.ProductId, result.ErrorMessage);
                throw new InvalidOperationException($"Failed to index product: {result.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing ProductCreatedEvent for product: {ProductId}", productEvent.ProductId);
            throw;
        }
    }

    private ProductDocument MapToProductDocument(ProductCreatedEvent productEvent)
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
            CreatedAt = productEvent.CreatedAt,
            UpdatedAt = productEvent.CreatedAt,
            Tags = productEvent.Tags,
            ImageUrls = productEvent.ImageUrls,
            Attributes = productEvent.Attributes,
            Slug = productEvent.Slug,
            Weight = productEvent.Weight
        };
    }
}
