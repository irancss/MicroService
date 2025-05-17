using ProductApi.Models.Entities;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using ProductApi.Infrastructure.Settings;
using MongoDB.Bson;

namespace ProductApi.Repositories
{
    public class MongoProductRepository : IProductRepository 
    {
        private readonly IMongoCollection<Product> _productsCollection;
        private readonly FilterDefinitionBuilder<Product> _filterBuilder = Builders<Product>.Filter;

        public MongoProductRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> mongoDbSettings)
        {
            var database = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _productsCollection = database.GetCollection<Product>(mongoDbSettings.Value.ProductsCollectionName);

            // --- ایجاد ایندکس‌ها (بهتر است یکبار در زمان استارت آپ انجام شود) ---
            var indexKeysDefinition = Builders<Product>.IndexKeys.Ascending(p => p.Sku);
            _productsCollection.Indexes.CreateOneAsync(new CreateIndexModel<Product>(indexKeysDefinition, new CreateIndexOptions { Unique = true }));

            var categoryIndex = Builders<Product>.IndexKeys.Ascending(p => p.Categories);
            _productsCollection.Indexes.CreateOneAsync(new CreateIndexModel<Product>(categoryIndex));

            var priceIndex = Builders<Product>.IndexKeys.Ascending(p => p.Price);
            _productsCollection.Indexes.CreateOneAsync(new CreateIndexModel<Product>(priceIndex));
            // ایندکس‌های دیگر بر اساس نیاز...
        }

        // متد CreateAsync دیگر StockQuantity و Discounts را مقداردهی نمی‌کند
        public async Task CreateAsync(Product product)
        {
            product.Id = null!;
            product.CreatedAt = product.UpdatedAt = DateTime.UtcNow;
            // اطمینان از صفر بودن مقادیر محاسبه‌ای در زمان ایجاد
            product.AverageRating = 0;
            product.ReviewCount = 0;
            await _productsCollection.InsertOneAsync(product);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            // Soft Delete:
            var filter = _filterBuilder.Eq(p => p.Id, id);
            var update = Builders<Product>.Update.Set(p => p.IsActive, false).Set(p => p.UpdatedAt, DateTime.UtcNow);
            var result = await _productsCollection.UpdateOneAsync(filter, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;

            // Hard Delete:
            // var filter = _filterBuilder.Eq(p => p.Id, id);
            // var result = await _productsCollection.DeleteOneAsync(filter);
            // return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<IEnumerable<Product>> GetAllAsync (string? lastId, int pageSize, string? category = null)
        {
           
            var filter = Builders<Product>.Filter.Eq(p => p.IsActive, true);
            if (!string.IsNullOrEmpty(lastId))
                filter &= Builders<Product>.Filter.Gt(p => p.Id, lastId);
            if (!string.IsNullOrEmpty(category))
                filter &= Builders<Product>.Filter.AnyEq(p => p.Categories, category);
           
            return await _productsCollection.Find(filter)
                .SortBy(p => p.Id)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<long> GetTotalCountAsync(string? category = null)
        {
            var filter = _filterBuilder.Eq(p => p.IsActive, true);
            if (!string.IsNullOrEmpty(category))
            {
                filter &= _filterBuilder.AnyEq(p => p.Categories, category);
            }
            // سایر فیلترها...
            return await _productsCollection.CountDocumentsAsync(filter);
        }


        public async Task<Product?> GetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out _)) return null; // بررسی معتبر بودن ObjectId
            var filter = _filterBuilder.Eq(p => p.Id, id) & _filterBuilder.Eq(p => p.IsActive, true);
            return await _productsCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Product?> GetBySkuAsync(string sku)
        {
            var filter = _filterBuilder.Eq(p => p.Sku, sku) & _filterBuilder.Eq(p => p.IsActive, true);
            return await _productsCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(string id, Product product)
        {
            if (!ObjectId.TryParse(id, out _)) return false;
            var filter = _filterBuilder.Eq(p => p.Id, id);
            // اطمینان از اینکه Id و CreatedAt آپدیت نمی‌شوند
            // product.Id = id; // Id نباید تغییر کند
            product.UpdatedAt = DateTime.UtcNow;

            // استفاده از ReplaceOneAsync تمام سند را جایگزین می‌کند (بجز _id)
            // var result = await _productsCollection.ReplaceOneAsync(filter, product);

            // ... (بخش فیلتر و بررسی ObjectId)
            var updateDefinition = Builders<Product>.Update
                .Set(p => p.Name, product.Name)
                .Set(p => p.Description, product.Description)
                .Set(p => p.Categories, product.Categories)
                .Set(p => p.Price, product.Price)
                .Set(p => p.Attributes, product.Attributes)
                .Set(p => p.IsActive, product.IsActive)
                .Set(p => p.UpdatedAt, DateTime.UtcNow);


            var result = await _productsCollection.UpdateOneAsync(_filterBuilder.Eq(p => p.Id, id), updateDefinition);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> AddMediaAsync(string productId, MediaInfo mediaInfo)
        {
            if (!ObjectId.TryParse(productId, out _)) return false;
            var filter = _filterBuilder.Eq(p => p.Id, productId);
            var update = Builders<Product>.Update.Push(p => p.Media, mediaInfo) // .Push آیتم را به آرایه اضافه می‌کند
                                           .Set(p => p.UpdatedAt, DateTime.UtcNow);
            var result = await _productsCollection.UpdateOneAsync(filter, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> RemoveMediaAsync(string productId, string mediaId)
        {
            if (!ObjectId.TryParse(productId, out _)) return false;
            var filter = _filterBuilder.Eq(p => p.Id, productId);
            var update = Builders<Product>.Update.PullFilter(p => p.Media, m => m.Id == mediaId) // PullFilter آیتم با شرط خاص را حذف می‌کند
                                           .Set(p => p.UpdatedAt, DateTime.UtcNow);
            var result = await _productsCollection.UpdateOneAsync(filter, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        // این متد همچنان وجود دارد اما توسط یک Background Worker/Consumer فراخوانی می‌شود
        // نه مستقیماً توسط ReviewService یا ProductService پس از افزودن نظر.
        public async Task UpdateRatingAsync(string productId, double newAverageRating, int newReviewCount)
        {
            if (!ObjectId.TryParse(productId, out _)) return;
            var filter = _filterBuilder.Eq(p => p.Id, productId);
            var update = Builders<Product>.Update
                .Set(p => p.AverageRating, newAverageRating)
                .Set(p => p.ReviewCount, newReviewCount)
                .Set(p => p.UpdatedAt, DateTime.UtcNow); // آپدیت تاریخ بروزرسانی
            await _productsCollection.UpdateOneAsync(filter, update);
        }

        public async Task<IEnumerable<Product>> SearchAsync(string query, int pageSize, string? lastId = null)
        {
            // Pseudocode:
            // 1. Build a filter that matches products where Name or Description contains the query (case-insensitive).
            // 2. Only include active products.
            // 3. If lastId is provided, only return products with Id > lastId (for pagination).
            // 4. Sort by Id ascending, limit to pageSize.
            // 5. Return the result as a list.

            var filters = new List<FilterDefinition<Product>>
            {
                _filterBuilder.Eq(p => p.IsActive, true)
            };

            if (!string.IsNullOrWhiteSpace(query))
            {
                var regex = new BsonRegularExpression(query, "i"); // case-insensitive
                var nameFilter = _filterBuilder.Regex(p => p.Name, regex);
                var descFilter = _filterBuilder.Regex(p => p.Description, regex);
                filters.Add(_filterBuilder.Or(nameFilter, descFilter));
            }

            if (!string.IsNullOrEmpty(lastId))
            {
                filters.Add(_filterBuilder.Gt(p => p.Id, lastId));
            }

            var filter = _filterBuilder.And(filters);

            return await _productsCollection.Find(filter)
                .SortBy(p => p.Id)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<string> ids)
        {
            // Pseudocode:
            // 1. Filter out invalid ObjectIds.
            // 2. Build a filter for products with Id in the valid ids and IsActive = true.
            // 3. Return the result as a list.

            var validIds = ids?.Where(id => ObjectId.TryParse(id, out _)).ToList() ?? new List<string>();
            if (!validIds.Any())
                return Enumerable.Empty<Product>();

            var filter = _filterBuilder.In(p => p.Id, validIds) & _filterBuilder.Eq(p => p.IsActive, true);
            return await _productsCollection.Find(filter).ToListAsync();
        }

        public async Task<bool> UpdateIsActiveAsync(string id, bool isActive)
        {
            // Pseudocode:
            // 1. Validate id as ObjectId.
            // 2. Update IsActive and UpdatedAt fields.
            // 3. Return true if update was acknowledged and modified at least one document.

            if (!ObjectId.TryParse(id, out _)) return false;
            var filter = _filterBuilder.Eq(p => p.Id, id);
            var update = Builders<Product>.Update
                .Set(p => p.IsActive, isActive)
                .Set(p => p.UpdatedAt, DateTime.UtcNow);
            var result = await _productsCollection.UpdateOneAsync(filter, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
    // Repository برای Review و Question نیز به همین ترتیب با Collection مربوطه پیاده‌سازی می‌شود.

}
