namespace ProductApi.Infrastructure.Kafka
{
    public interface IKafkaProducerService : IDisposable // IDisposable برای بستن Producer
    {
        Task ProduceAsync<T>(string topic, T message);
    }
}
