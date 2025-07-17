using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Logging;
using SearchService.Domain.Entities;
using SearchService.Domain.Interfaces;

namespace SearchService.Infrastructure.Repositories;

public class ElasticsearchRepository : ISearchRepository
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticsearchRepository> _logger;
    private const string ProductIndexName = "products";

    public ElasticsearchRepository(ElasticsearchClient client, ILogger<ElasticsearchRepository> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<bool> IndexProductAsync(ProductDocument product, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.IndexAsync(product, idx => idx.Index(ProductIndexName).Id(product.Id), cancellationToken);
            
            if (response.IsValidResponse)
            {
                _logger.LogInformation("Successfully indexed product: {ProductId}", product.Id);
                return true;
            }

            _logger.LogError("Failed to index product {ProductId}: {Error}", product.Id, response.ElasticsearchServerError?.Error?.Reason);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while indexing product: {ProductId}", product.Id);
            return false;
        }
    }

    public async Task<bool> UpdateProductAsync(string productId, ProductDocument product, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.UpdateAsync<ProductDocument, ProductDocument>(ProductIndexName, productId, 
                u => u.Doc(product).DocAsUpsert(true), cancellationToken);

            if (response.IsValidResponse)
            {
                _logger.LogInformation("Successfully updated product: {ProductId}", productId);
                return true;
            }

            _logger.LogError("Failed to update product {ProductId}: {Error}", productId, response.ElasticsearchServerError?.Error?.Reason);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while updating product: {ProductId}", productId);
            return false;
        }
    }

    public async Task<bool> DeleteProductAsync(string productId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.DeleteAsync(ProductIndexName, productId, cancellationToken);

            if (response.IsValidResponse)
            {
                _logger.LogInformation("Successfully deleted product: {ProductId}", productId);
                return true;
            }

            _logger.LogError("Failed to delete product {ProductId}: {Error}", productId, response.ElasticsearchServerError?.Error?.Reason);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while deleting product: {ProductId}", productId);
            return false;
        }
    }

    public async Task<bool> BulkIndexProductsAsync(IEnumerable<ProductDocument> products, CancellationToken cancellationToken = default)
    {
        try
        {
            var bulkRequest = new BulkRequestDescriptor();

            foreach (var product in products)
            {
                bulkRequest.Index<ProductDocument>(product, idx => idx
                    .Index(ProductIndexName)
                    .Id(product.Id));
            }

            var response = await _client.BulkAsync(bulkRequest, cancellationToken);

            if (response.IsValidResponse)
            {
                _logger.LogInformation("Successfully bulk indexed {Count} products", products.Count());
                return true;
            }

            _logger.LogError("Bulk indexing failed: {Error}", response.ElasticsearchServerError?.Error?.Reason);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred during bulk indexing");
            return false;
        }
    }

    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.PingAsync(cancellationToken);
            return response.IsValidResponse;
        }
        catch
        {
            return false;
        }
    }

    public async Task<ProductDocument?> GetProductByIdAsync(string productId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.GetAsync<ProductDocument>(ProductIndexName, productId, cancellationToken);

            if (response.IsValidResponse && response.Found)
            {
                return response.Source;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while getting product: {ProductId}", productId);
            return null;
        }
    }
}
