using MediatR;
using Microsoft.Extensions.Logging;
using SearchService.Domain.Interfaces;

namespace SearchService.Application.Commands.RemoveProduct;

public class RemoveProductFromIndexCommandHandler : IRequestHandler<RemoveProductFromIndexCommand, RemoveProductFromIndexResponse>
{
    private readonly ISearchRepository _searchRepository;
    private readonly ILogger<RemoveProductFromIndexCommandHandler> _logger;

    public RemoveProductFromIndexCommandHandler(
        ISearchRepository searchRepository,
        ILogger<RemoveProductFromIndexCommandHandler> logger)
    {
        _searchRepository = searchRepository;
        _logger = logger;
    }

    public async Task<RemoveProductFromIndexResponse> Handle(RemoveProductFromIndexCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Removing product from index: {ProductId}", request.ProductId);

            var success = await _searchRepository.DeleteProductAsync(request.ProductId, cancellationToken);

            if (success)
            {
                _logger.LogInformation("Successfully removed product from index: {ProductId}", request.ProductId);
                return new RemoveProductFromIndexResponse
                {
                    Success = true,
                    ProductId = request.ProductId
                };
            }

            _logger.LogWarning("Failed to remove product from index: {ProductId}", request.ProductId);
            return new RemoveProductFromIndexResponse
            {
                Success = false,
                ErrorMessage = "Failed to remove product from index",
                ProductId = request.ProductId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing product from index: {ProductId}", request.ProductId);
            return new RemoveProductFromIndexResponse
            {
                Success = false,
                ErrorMessage = ex.Message,
                ProductId = request.ProductId
            };
        }
    }
}
