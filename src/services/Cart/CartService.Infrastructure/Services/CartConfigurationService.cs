using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Newtonsoft.Json;
using Cart.Application.Interfaces;

namespace Cart.Infrastructure.Services;

public class CartConfigurationService : ICartConfigurationService
{
    private readonly IDatabase _database;
    private readonly ILogger<CartConfigurationService> _logger;
    private readonly JsonSerializerSettings _jsonSettings;
    private const string ConfigKey = "cart:configuration";

    public CartConfigurationService(IConnectionMultiplexer redis, ILogger<CartConfigurationService> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
        _jsonSettings = new JsonSerializerSettings
        {
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task<CartConfiguration> GetConfigurationAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var configJson = await _database.StringGetAsync(ConfigKey);
            
            if (!configJson.HasValue)
            {
                // Return default configuration
                var defaultConfig = new CartConfiguration();
                await UpdateConfigurationAsync(defaultConfig);
                return defaultConfig;
            }

            var config = JsonConvert.DeserializeObject<CartConfiguration>(configJson!, _jsonSettings);
            return config ?? new CartConfiguration();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart configuration, returning default");
            return new CartConfiguration();
        }
    }

    public async Task UpdateConfigurationAsync(CartConfiguration configuration, CancellationToken cancellationToken = default)
    {
        try
        {
            var configJson = JsonConvert.SerializeObject(configuration, _jsonSettings);
            await _database.StringSetAsync(ConfigKey, configJson);
            
            _logger.LogInformation("Updated cart configuration");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cart configuration");
            throw;
        }
    }
}
