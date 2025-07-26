using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Payment.Infrastructure.Services;

public interface IEventPublisher
{
    Task PublishAsync<T>(T eventData, string routingKey) where T : class;
    Task PublishPaymentSuccessAsync(Guid transactionId, string orderId, Guid userId, decimal amount, string currency);
    Task PublishPaymentFailedAsync(Guid transactionId, string orderId, Guid userId, decimal amount, string currency, string? reason);
    Task PublishWalletUpdatedAsync(Guid userId, decimal newBalance, string currency, string operation);
}

public class RabbitMQEventPublisher : IEventPublisher, IDisposable
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
                DispatchConsumersAsync = true // افزودن این برای سازگاری بهتر با async consumer ها خوب است
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(
                exchange: _exchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments: null);

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

            // --- START OF CORRECTION 1 ---
            // متد CreateBasicProperties تغییری نکرده اما استفاده از آن در BasicPublish تغییر کرده
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.MessageId = Guid.NewGuid().ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            properties.Type = typeof(T).Name;

            // نام پارامتر از basicProperties به properties تغییر کرده است
            _channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: routingKey,
               // properties: properties, // <-- اصلاح اصلی اینجاست
                body: body);
            // --- END OF CORRECTION 1 ---

            _logger.LogDebug("Event published: {EventType}, RoutingKey: {RoutingKey}", typeof(T).Name, routingKey);

            await Task.CompletedTask; // این بخش برای یک متد Publisher می‌تواند بهینه شود
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
            // --- START OF CORRECTION 2 ---
            // متدهای Close() حذف شده‌اند. Dispose() برای بستن و آزادسازی منابع کافی است.
            _channel?.Dispose();
            _connection?.Dispose();
            // --- END OF CORRECTION 2 ---
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
