using BuildingBlocks.Infrastructure.Caching;
using Cart.Application.Interfaces;
using Cart.Domain.Entities;

namespace Cart.Infrastructure.Repositories
{
    public class ActiveCartRepository : IActiveCartRepository
    {
        private readonly IDistributedCacheService _cache;
        private readonly TimeSpan _cartExpiration = TimeSpan.FromMinutes(60); // Default, will be overridden by config

        public ActiveCartRepository(IDistributedCacheService cache)
        {
            _cache = cache;
        }

        private static string GetKey(string cartId) => $"cart:active:{cartId}";

        public Task<ActiveCart?> GetByIdAsync(string cartId, CancellationToken cancellationToken = default)
        {
            return _cache.GetAsync<ActiveCart>(GetKey(cartId), cancellationToken);
        }

        public async Task<ActiveCart> SaveAsync(ActiveCart cart, CancellationToken cancellationToken = default)
        {
            // TODO: Get expiration from CartConfiguration
            await _cache.SetAsync(GetKey(cart.Id), cart, _cartExpiration, cancellationToken: cancellationToken);
            return cart;
        }

        public Task DeleteAsync(string cartId, CancellationToken cancellationToken = default)
        {
            return _cache.RemoveAsync(GetKey(cartId), cancellationToken);
        }
    }
}
