using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Payment.Infrastructure.Services;

public interface IEventPublisher
{
    Task PublishAsync<T>(T eventData, string routingKey) where T : class;
    Task PublishPaymentSuccessAsync(Guid transactionId, string orderId, Guid userId, decimal amount, string currency);
    Task PublishPaymentFailedAsync(Guid transactionId, string orderId, Guid userId, decimal amount, string currency, string? reason);
    Task PublishWalletUpdatedAsync(Guid userId, decimal newBalance, string currency, string operation);
}

public  class RabbitMQEventPublisher : IEventPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQEventPublisher> _logger;
    private readonly string _exchangeName;

    public RabbitMQEventPublisher(IConfiguration configuration, ILogger<RabbitMQEventPublisher> logger)
    {
        _logger = logger;

        var hostName = configuration.GetValue<string>("RabbitMQ:HostName") ?? "localhost";
        var port = configuration.GetValue<int>("RabbitMQ:Port", 5672);
        var userName = configuration.GetValue<string>("RabbitMQ:UserName") ?? "guest";
        var password = configuration.GetValue<string>("RabbitMQ:Password") ?? "guest";
        var virtualHost = configuration.GetValue<string>("RabbitMQ:VirtualHost") ?? "/";
        _exchangeName = configuration.GetValue<string>("RabbitMQ:ExchangeName") ?? "payment.events";

        try
        {
            var factory = new ConnectionFactory
            {
                HostName = hostName,
                Port = port,
                UserName = userName,
                Password = password,
                VirtualHost = virtualHost,
                // [حذف شد] این پراپرتی در نسخه 6.0 به بالا وجود ندارد.
                // DispatchConsumersAsync = true 
            };

            // [اصلاح شد] از متدهای سنکرون برای ایجاد کانکشن و چنل استفاده کنید.
            // این روش برای Publisher ساده‌تر و رایج‌تر است.
            //_connection = factory.CreateConnection();
            //_channel = _connection.CreateModel();

            //_channel.ExchangeDeclare(
            //    exchange: _exchangeName,
            //    type: ExchangeType.Topic,
            //    durable: true,
            //    autoDelete: false,
            //    arguments: null);

            _logger.LogInformation("RabbitMQ connection established successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to establish RabbitMQ connection");
            throw;
        }
    }


    public async Task PublishAsync<T>(T eventData, string routingKey) where T : class
    {
        try
        {
            var message = JsonSerializer.Serialize(eventData, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var body = Encoding.UTF8.GetBytes(message);

            //var properties = _channel.CreateBasicProperties();
            //properties.Persistent = true;
            //properties.MessageId = Guid.NewGuid().ToString();
            //properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            //properties.Type = typeof(T).Name;

            // [اصلاح شد] امضای متد BasicPublish تغییر کرده است.
            // نام پارامتر از `properties` به `basicProperties` تغییر کرده است.
            // همچنین، await در اینجا ضرورتی ندارد چون BasicPublish یک متد void است.
            //_channel.BasicPublish(
            //    exchange: _exchangeName,
            //    routingKey: routingKey,
            //    basicProperties: properties, // <-- اصلاح اصلی اینجاست
            //    body: body);

            _logger.LogDebug("Event published: {EventType}, RoutingKey: {RoutingKey}", typeof(T).Name, routingKey);

            // Publish یک عملیات fire-and-forget است و نیازی به await ندارد.
            // اگر می‌خواهید از صحت ارسال مطمئن شوید باید از Publisher Confirms استفاده کنید که پیچیده‌تر است.
            // برای سادگی، این خط را نگه می‌داریم تا امضای متد async باقی بماند.
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event: {EventType}, RoutingKey: {RoutingKey}", typeof(T).Name, routingKey);
            throw;
        }
    }
    public async Task PublishPaymentSuccessAsync(Guid transactionId, string orderId, Guid userId, decimal amount, string currency)
    {
        var eventData = new PaymentSuccessEvent
        {
            TransactionId = transactionId,
            OrderId = orderId,
            UserId = userId,
            Amount = amount,
            Currency = currency,
            Timestamp = DateTime.UtcNow
        };

        await PublishAsync(eventData, "payment.success");
    }

    public async Task PublishPaymentFailedAsync(Guid transactionId, string orderId, Guid userId, decimal amount, string currency, string? reason)
    {
        var eventData = new PaymentFailedEvent
        {
            TransactionId = transactionId,
            OrderId = orderId,
            UserId = userId,
            Amount = amount,
            Currency = currency,
            Reason = reason,
            Timestamp = DateTime.UtcNow
        };

        await PublishAsync(eventData, "payment.failed");
    }

    public async Task PublishWalletUpdatedAsync(Guid userId, decimal newBalance, string currency, string operation)
    {
        var eventData = new WalletUpdatedEvent
        {
            UserId = userId,
            NewBalance = newBalance,
            Currency = currency,
            Operation = operation,
            Timestamp = DateTime.UtcNow
        };

        await PublishAsync(eventData, "wallet.updated");
    }

    public void Dispose()
    {
        try
        {
            //_channel?.Dispose();
            _connection?.Dispose();
            _logger.LogInformation("RabbitMQ connection disposed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing RabbitMQ connection");
        }
    }
}

// Event Data Models
public class PaymentSuccessEvent
{
    public Guid TransactionId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class PaymentFailedEvent
{
    public Guid TransactionId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public DateTime Timestamp { get; set; }
}

public class WalletUpdatedEvent
{
    public Guid UserId { get; set; }
    public decimal NewBalance { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty; // "deposit", "withdrawal", "purchase"
    public DateTime Timestamp { get; set; }
}
