using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using Cart.Application.Interfaces;
using Cart.Domain.Entities;

namespace Cart.Infrastructure.Repositories;

public class RedisCartRepository : ICartRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<RedisCartRepository> _logger;
    private readonly JsonSerializerSettings _jsonSettings;

    public RedisCartRepository(IConnectionMultiplexer redis, ILogger<RedisCartRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
        _jsonSettings = new JsonSerializerSettings
        {
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task<ShoppingCart?> GetByUserIdAsync(string userId)
    {
        try
        {
            var key = $"cart:user:{userId}";
            var cartJson = await _database.StringGetAsync(key);
            
            if (!cartJson.HasValue)
                return null;

            var cart = JsonConvert.DeserializeObject<ShoppingCart>(cartJson!, _jsonSettings);
            return cart;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart for user {UserId}", userId);
            return null;
        }
    }

    public async Task<ShoppingCart?> GetByGuestIdAsync(string guestId)
    {
        try
        {
            var key = $"cart:guest:{guestId}";
            var cartJson = await _database.StringGetAsync(key);
            
            if (!cartJson.HasValue)
                return null;

            var cart = JsonConvert.DeserializeObject<ShoppingCart>(cartJson!, _jsonSettings);
            return cart;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart for guest {GuestId}", guestId);
            return null;
        }
    }

    public async Task<ShoppingCart?> GetByIdAsync(string cartId)
    {
        try
        {
            // Try both user and guest patterns since we don't know the type
            var userKey = $"cart:user:{cartId}";
            var guestKey = $"cart:guest:{cartId}";

            var userCartJson = await _database.StringGetAsync(userKey);
            if (userCartJson.HasValue)
            {
                return JsonConvert.DeserializeObject<ShoppingCart>(userCartJson!, _jsonSettings);
            }

            var guestCartJson = await _database.StringGetAsync(guestKey);
            if (guestCartJson.HasValue)
            {
                return JsonConvert.DeserializeObject<ShoppingCart>(guestCartJson!, _jsonSettings);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart by ID {CartId}", cartId);
            return null;
        }
    }

    public async Task SaveAsync(ShoppingCart cart)
    {
        try
        {
            var key = cart.GetCartKey();
            var cartJson = JsonConvert.SerializeObject(cart, _jsonSettings);
            
            // Set expiration to 30 days
            await _database.StringSetAsync(key, cartJson, TimeSpan.FromDays(30));
            
            _logger.LogDebug("Saved cart {CartId} with key {Key}", cart.Id, key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving cart {CartId}", cart.Id);
            throw;
        }
    }

    public async Task DeleteAsync(string cartId)
    {
        try
        {
            // Try to delete both possible keys
            var userKey = $"cart:user:{cartId}";
            var guestKey = $"cart:guest:{cartId}";

            await Task.WhenAll(
                _database.KeyDeleteAsync(userKey),
                _database.KeyDeleteAsync(guestKey)
            );

            _logger.LogDebug("Deleted cart {CartId}", cartId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting cart {CartId}", cartId);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string cartId)
    {
        try
        {
            var userKey = $"cart:user:{cartId}";
            var guestKey = $"cart:guest:{cartId}";

            var userExists = await _database.KeyExistsAsync(userKey);
            var guestExists = await _database.KeyExistsAsync(guestKey);

            return userExists || guestExists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if cart exists {CartId}", cartId);
            return false;
        }
    }

    public async Task<List<ShoppingCart>> GetAbandonedCartsAsync(DateTime abandonmentThreshold)
    {
        try
        {
            var abandonedCarts = new List<ShoppingCart>();
            
            // Get all cart keys with the pattern
            var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
            var userKeys = server.Keys(pattern: "cart:user:*");
            var guestKeys = server.Keys(pattern: "cart:guest:*");
            
            var allKeys = userKeys.Concat(guestKeys);

            foreach (var key in allKeys)
            {
                var cartJson = await _database.StringGetAsync(key);
                if (cartJson.HasValue)
                {
                    var cart = JsonConvert.DeserializeObject<ShoppingCart>(cartJson!, _jsonSettings);
                    if (cart != null && 
                        cart.HasActiveItems() && 
                        cart.LastModifiedUtc < abandonmentThreshold &&
                        !cart.IsAbandoned)
                    {
                        abandonedCarts.Add(cart);
                    }
                }
            }

            return abandonedCarts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting abandoned carts");
            return new List<ShoppingCart>();
        }
    }

    public async Task<List<ShoppingCart>> GetExpiredCartsAsync(DateTime expirationThreshold)
    {
        try
        {
            var expiredCarts = new List<ShoppingCart>();
            
            var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
            var userKeys = server.Keys(pattern: "cart:user:*");
            var guestKeys = server.Keys(pattern: "cart:guest:*");
            
            var allKeys = userKeys.Concat(guestKeys);

            foreach (var key in allKeys)
            {
                var cartJson = await _database.StringGetAsync(key);
                if (cartJson.HasValue)
                {
                    var cart = JsonConvert.DeserializeObject<ShoppingCart>(cartJson!, _jsonSettings);
                    if (cart != null && cart.LastModifiedUtc < expirationThreshold)
                    {
                        expiredCarts.Add(cart);
                    }
                }
            }

            return expiredCarts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting expired carts");
            return new List<ShoppingCart>();
        }
    }
}
