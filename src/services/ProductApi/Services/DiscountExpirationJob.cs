using MongoDB.Driver;
using ProductApi.Models.Entities;

namespace ProductApi.Services
{
    public class DiscountExpirationJob : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var filter = Builders<Product>.Filter.ElemMatch(p => p.ScheduledDiscounts, d => d.IsActive && d.EndDate < now);
                var update = Builders<Product>.Update.Set("ScheduledDiscounts.$[elem].IsActive", false);
                await _productsCollection.UpdateManyAsync(filter, update, cancellationToken: stoppingToken);
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
