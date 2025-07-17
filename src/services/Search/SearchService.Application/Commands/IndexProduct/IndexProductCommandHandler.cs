using MediatR;
using Microsoft.Extensions.Logging;
using SearchService.Domain.Interfaces;

namespace SearchService.Application.Commands.IndexProduct;

public class IndexProductCommandHandler : IRequestHandler<IndexProductCommand, IndexProductResponse>
{
    private readonly ISearchRepository _searchRepository;
    private readonly ILogger<IndexProductCommandHandler> _logger;

    public IndexProductCommandHandler(
        ISearchRepository searchRepository,
        ILogger<IndexProductCommandHandler> logger)
    {
        _searchRepository = searchRepository;
        _logger = logger;
    }

    public async Task<IndexProductResponse> Handle(IndexProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Indexing product with ID: {ProductId}", request.Product.Id);

            // Set timestamps
            var now = DateTime.UtcNow;
            request.Product.UpdatedAt = now;
            if (request.Product.CreatedAt == default)
                request.Product.CreatedAt = now;

            var success = await _searchRepository.IndexProductAsync(request.Product, cancellationToken);

            if (success)
            {
                _logger.LogInformation("Successfully indexed product: {ProductId}", request.Product.Id);
                return new IndexProductResponse
                {
                    Success = true,
                    ProductId = request.Product.Id
                };
            }

            _logger.LogWarning("Failed to index product: {ProductId}", request.Product.Id);
            return new IndexProductResponse
            {
                Success = false,
                ErrorMessage = "Failed to index product",
                ProductId = request.Product.Id
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing product: {ProductId}", request.Product.Id);
            return new IndexProductResponse
            {
                Success = false,
                ErrorMessage = ex.Message,
                ProductId = request.Product.Id
            };
        }
    }
}
