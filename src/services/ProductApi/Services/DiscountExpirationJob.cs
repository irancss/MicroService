using MongoDB.Bson;
using MongoDB.Driver;
using ProductApi.Models.Entities;

namespace ProductApi.Services
{
    public class DiscountExpirationJob : BackgroundService
    {
        private readonly IMongoCollection<Product> _productsCollection;

        public DiscountExpirationJob(IMongoCollection<Product> productsCollection)
        {
            _productsCollection = productsCollection;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                // Use BsonDocument for filter and array filter since ScheduledDiscount is not available
                var filter = Builders<Product>.Filter.ElemMatch("ScheduledDiscounts", Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("IsActive", true),
                    Builders<BsonDocument>.Filter.Lt("EndDate", now)
                ));
                var update = Builders<Product>.Update.Set("ScheduledDiscounts.$[elem].IsActive", false);
                var arrayFilters = new List<ArrayFilterDefinition>
                    {
                        new JsonArrayFilterDefinition<BsonDocument>("{ 'elem.IsActive': true, 'elem.EndDate': { '$lt': ISODate('" + now.ToString("o") + "') } }")
                    };
                var updateOptions = new UpdateOptions { ArrayFilters = arrayFilters };

                await _productsCollection.UpdateManyAsync(filter, update, updateOptions, stoppingToken);
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
