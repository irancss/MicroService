using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Text.Json;

namespace Payment.Infrastructure.Services;

public interface ILogService
{
    Task LogGatewayRequestAsync(string gatewayName, string orderId, object request);
    Task LogGatewayResponseAsync(string gatewayName, string orderId, object response, bool isSuccess);
    Task<IEnumerable<GatewayLog>> GetLogsAsync(string? gatewayName = null, string? orderId = null, DateTime? from = null, DateTime? to = null);
}

public class MongoLogService : ILogService
{
    private readonly IMongoCollection<GatewayLog> _logsCollection;
    private readonly ILogger<MongoLogService> _logger;

    public MongoLogService(IConfiguration configuration, ILogger<MongoLogService> logger)
    {
        _logger = logger;
        
        var connectionString = configuration.GetConnectionString("MongoDB") ?? 
            throw new ArgumentException("MongoDB connection string not found");
        
        var databaseName = configuration.GetValue<string>("MongoDB:DatabaseName") ?? "PaymentLogs";
        
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _logsCollection = database.GetCollection<GatewayLog>("gateway_logs");

        // Create indexes
        CreateIndexes();
    }

    public async Task LogGatewayRequestAsync(string gatewayName, string orderId, object request)
    {
        try
        {
            var log = new GatewayLog
            {
                Id = Guid.NewGuid().ToString(),
                GatewayName = gatewayName,
                OrderId = orderId,
                Type = "Request",
                Data = JsonSerializer.Serialize(request),
                Timestamp = DateTime.UtcNow,
                IsSuccess = null // Request doesn't have success/failure
            };

            await _logsCollection.InsertOneAsync(log);
            _logger.LogDebug("Gateway request logged for {GatewayName}, Order: {OrderId}", gatewayName, orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log gateway request for {GatewayName}, Order: {OrderId}", gatewayName, orderId);
        }
    }

    public async Task LogGatewayResponseAsync(string gatewayName, string orderId, object response, bool isSuccess)
    {
        try
        {
            var log = new GatewayLog
            {
                Id = Guid.NewGuid().ToString(),
                GatewayName = gatewayName,
                OrderId = orderId,
                Type = "Response",
                Data = JsonSerializer.Serialize(response),
                Timestamp = DateTime.UtcNow,
                IsSuccess = isSuccess
            };

            await _logsCollection.InsertOneAsync(log);
            _logger.LogDebug("Gateway response logged for {GatewayName}, Order: {OrderId}, Success: {IsSuccess}", 
                gatewayName, orderId, isSuccess);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log gateway response for {GatewayName}, Order: {OrderId}", gatewayName, orderId);
        }
    }

    public async Task<IEnumerable<GatewayLog>> GetLogsAsync(string? gatewayName = null, string? orderId = null, DateTime? from = null, DateTime? to = null)
    {
        try
        {
            var filterBuilder = Builders<GatewayLog>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(gatewayName))
                filter &= filterBuilder.Eq(x => x.GatewayName, gatewayName);

            if (!string.IsNullOrEmpty(orderId))
                filter &= filterBuilder.Eq(x => x.OrderId, orderId);

            if (from.HasValue)
                filter &= filterBuilder.Gte(x => x.Timestamp, from.Value);

            if (to.HasValue)
                filter &= filterBuilder.Lte(x => x.Timestamp, to.Value);

            return await _logsCollection
                .Find(filter)
                .SortByDescending(x => x.Timestamp)
                .Limit(1000) // Limit to prevent large responses
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get gateway logs");
            return Enumerable.Empty<GatewayLog>();
        }
    }

    private void CreateIndexes()
    {
        try
        {
            var indexModels = new List<CreateIndexModel<GatewayLog>>
            {
                new(Builders<GatewayLog>.IndexKeys.Ascending(x => x.GatewayName)),
                new(Builders<GatewayLog>.IndexKeys.Ascending(x => x.OrderId)),
                new(Builders<GatewayLog>.IndexKeys.Descending(x => x.Timestamp)),
                new(Builders<GatewayLog>.IndexKeys.Combine(
                    Builders<GatewayLog>.IndexKeys.Ascending(x => x.GatewayName),
                    Builders<GatewayLog>.IndexKeys.Ascending(x => x.OrderId)))
            };

            _logsCollection.Indexes.CreateMany(indexModels);
            _logger.LogDebug("MongoDB indexes created for gateway logs");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to create MongoDB indexes");
        }
    }
}

public class GatewayLog
{
    public string Id { get; set; } = string.Empty;
    public string GatewayName { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "Request" or "Response"
    public string Data { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public bool? IsSuccess { get; set; }
}
