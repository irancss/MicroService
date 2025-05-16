namespace ProductApi.Infrastructure.Settings
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string ProductsCollectionName { get; set; } = null!;
        public string ReviewsCollectionName { get; set; } = null!;
        public string QuestionsCollectionName { get; set; } = null!;
    }
}
