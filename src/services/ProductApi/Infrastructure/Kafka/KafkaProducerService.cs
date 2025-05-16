using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using ProductApi.Infrastructure.Settings;

namespace ProductApi.Infrastructure.Kafka;

public class KafkaProducerService : IKafkaProducerService
{
    private readonly IProducer<Null, string> _producer;
    private readonly ILogger<KafkaProducerService> _logger;
    private readonly KafkaSettings _kafkaSettings; // برای دسترسی به نام تاپیک‌ها (اختیاری)

    public KafkaProducerService(IOptions<KafkaSettings> kafkaOptions, ILogger<KafkaProducerService> logger)
    {
        _logger = logger;
        _kafkaSettings = kafkaOptions.Value; // ذخیره تنظیمات در صورت نیاز

        var config = new ProducerConfig { BootstrapServers = _kafkaSettings.BootstrapServers };
        // تنظیمات دیگر مانند فشرده‌سازی، ACK و ...

        // ایجاد Producer به صورت Singleton توصیه می‌شود (در DI ثبت شود)
        try
        {
            _producer = new ProducerBuilder<Null, string>(config).Build();
            _logger.LogInformation("Kafka producer built successfully for servers: {BootstrapServers}", _kafkaSettings.BootstrapServers);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to build Kafka producer for servers: {BootstrapServers}", _kafkaSettings.BootstrapServers);
            // در این حالت برنامه نباید ادامه دهد یا باید مکانیزم جایگزین فعال شود
            throw;
        }
    }

    public async Task ProduceAsync<T>(string topic, T message)
    {
        if (message == null)
        {
            _logger.LogWarning("Attempted to produce a null message to topic {Topic}", topic);
            return;
        }

        try
        {
            var serializedMessage = JsonSerializer.Serialize(message);
            _logger.LogDebug("Producing message to Kafka topic '{Topic}': {Message}", topic, serializedMessage);

            // ارسال پیام به Kafka. Key می‌تواند Null باشد یا شناسه‌ای مانند ProductId برای پارتیشن‌بندی بهتر
            // var result = await _producer.ProduceAsync(topic, new Message<Null, string> { Value = serializedMessage });

            // استفاده از ProductId به عنوان Key برای اطمینان از رفتن تمام رویدادهای یک محصول به یک پارتیشن
            string? messageKey = null;
            if (message is ProductChangeEvent pce) messageKey = pce.ProductId;
            // استخراج Key برای انواع دیگر پیام‌ها...

            var result = await _producer.ProduceAsync(topic,
                new Message<string, string> { Key = messageKey!, Value = serializedMessage }); // Note: Key can be null


            _logger.LogInformation(
                "Message delivered to Kafka topic '{Topic}' [Partition: {Partition}, Offset: {Offset}]", topic,
                result.Partition, result.Offset);
        }
        catch (ProduceException<Null, string> e)
        {
            _logger.LogError(e, "Kafka delivery failed for topic '{Topic}': {Reason}", topic, e.Error.Reason);
            // اینجا می‌توان پیام را در صف دیگری ذخیره کرد یا مکانیزم Retry پیاده‌سازی کرد
        }
        catch (JsonException jsonEx)
        {
            _logger.LogError(jsonEx,
                "Failed to serialize Kafka message for topic '{Topic}'. Message Type: {MessageType}", topic,
                typeof(T).FullName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while producing Kafka message to topic '{Topic}'",
                topic);
        }

        Dispose();
    }

    public void Dispose()
    {
        // Flush پیام‌های باقیمانده در بافر قبل از بستن
        _producer.Flush(TimeSpan.FromSeconds(10));
        _producer.Dispose();
        _logger.LogInformation("Kafka producer disposed.");
    }
}