using BuildingBlocks.Messaging;
using Cart.Application.Interfaces;
using Cart.Infrastructure.GrpcClients;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Cart.Infrastructure.BackgroundJobs;

// This service is a simplified replacement for Hangfire
public class ActiveCartExpirationService : BackgroundService
{
    private readonly ILogger<ActiveCartExpirationService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ActiveCartExpirationService(ILogger<ActiveCartExpirationService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Active Cart Expiration Service is starting.");
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Check every 5 minutes
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var configService = scope.ServiceProvider.GetRequiredService<ICartConfigurationService>();
                var redis = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();
                var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

                var config = await configService.GetConfigurationAsync(stoppingToken);
                var database = redis.GetDatabase();
                var server = redis.GetServer(redis.GetEndPoints().First());

                var keys = server.Keys(pattern: "cart:active:*");
                foreach (var key in keys)
                {
                    if (stoppingToken.IsCancellationRequested) break;

                    var cartJson = await database.StringGetAsync(key);
                    if (cartJson.IsNullOrEmpty) continue;

                    var cart = System.Text.Json.JsonSerializer.Deserialize<Domain.Entities.ActiveCart>(cartJson!);
                    if (cart is null) continue;

                    if ((DateTime.UtcNow - cart.LastModifiedUtc).TotalMinutes > config.ActiveCartExpiryMinutes)
                    {
                        _logger.LogWarning("Cart {CartId} has expired. Releasing stock.", cart.Id);

                        var releaseCommand = new ReleaseStockCommand(cart.Id, "Cart expired");
                        await messageBus.SendAsync(releaseCommand, stoppingToken);
                        await database.KeyDeleteAsync(key);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the cart expiration service.");
            }
        }
    }
}