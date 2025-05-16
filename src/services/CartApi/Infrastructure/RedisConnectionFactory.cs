using CartApi.Services;
using Confluent.Kafka;

namespace CartApi.Infrastructure
{
    public class RedisConnectionFactory
    {
    }

    public class KafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;
        public KafkaProducerService(ProducerConfig config)
        {
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task ProduceAsync(string topic, string message)
        {
            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
        }
    }

    public class KafkaConsumerService : BackgroundService
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly ICartService _cartService;
        public KafkaConsumerService(ConsumerConfig config, ICartService cartService)
        {
            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            _cartService = cartService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe("order-completed");

            while (!stoppingToken.IsCancellationRequested)
            {
                var result = _consumer.Consume(stoppingToken);
                var userId = result.Message.Value;
                await _cartService.ClearCartAsync(userId);
            }
        }
    }
}
