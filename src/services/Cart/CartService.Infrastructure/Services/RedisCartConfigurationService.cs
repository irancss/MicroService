using BuildingBlocks.Infrastructure.Caching;
using Cart.Application.Interfaces;

namespace Cart.Infrastructure.Services
{
    public class RedisCartConfigurationService : ICartConfigurationService
    {
        private readonly IDistributedCacheService _cache;
        private const string ConfigKey = "cart:configuration";

        public RedisCartConfigurationService(IDistributedCacheService cache)
        {
            _cache = cache;
        }

        public async Task<CartConfiguration> GetConfigurationAsync(CancellationToken cancellationToken = default)
        {
            var config = await _cache.GetAsync<CartConfiguration>(ConfigKey, cancellationToken);
            if (config is null)
            {
                config = new CartConfiguration();
                await UpdateConfigurationAsync(config, cancellationToken);
            }
            return config;
        }

        public Task UpdateConfigurationAsync(CartConfiguration configuration, CancellationToken cancellationToken = default)
        {
            return _cache.SetAsync(ConfigKey, configuration, cancellationToken: cancellationToken);
        }
    }
}
