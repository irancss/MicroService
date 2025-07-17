using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MassTransit;
using BuildingBlocks.Messaging.Events;

namespace BuildingBlocks.Messaging.EventDriven
{
    /// <summary>
    /// Event-driven architecture patterns and utilities
    /// </summary>
    public static class EventDrivenExtensions
    {
        /// <summary>
        /// Add event-driven capabilities with automatic consumer discovery
        /// </summary>
       public static IServiceCollection AddEventDrivenArchitecture(
    this IServiceCollection services, 
    IConfiguration configuration,
    params Type[] consumerAssemblyMarkers)
{
    services.AddMassTransit(x =>
    {
                // Automatically discover and register all consumers from specified assemblies
                foreach (var markerType in consumerAssemblyMarkers)
                {
                    x.AddConsumersFromNamespaceContaining(markerType);
                }

                

                 x.AddSagaStateMachine<OrderStateMachine, OrderState>()
            .EntityFrameworkCoreRepository(r =>
            {
                r.AddDbContext<DbContext, SagaDbContext>((provider, builder) =>
                {
                    // کانکشن استرینگ دیتابیس Saga ها
                    var connectionString = configuration.GetConnectionString("SagaDatabase");
                    builder.UseNpgsql(connectionString, m =>
                    {
                        m.MigrationsAssembly(typeof(SagaDbContext).Assembly.FullName);
                    });
                });
                r.ConcurrencyMode = ConcurrencyMode.Pessimistic; // برای جلوگیری از تداخل
            });
            
                // Configure RabbitMQ
        x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMqSettings = configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>();
                    
                    cfg.Host(rabbitMqSettings?.Host ?? "localhost", rabbitMqSettings?.VirtualHost ?? "/", h =>
                    {
                        h.Username(rabbitMqSettings?.Username ?? "guest");
                        h.Password(rabbitMqSettings?.Password ?? "guest");
                    });

                    // Configure message topology
                    ConfigureMessageTopology(cfg);

                    // Configure endpoints with naming conventions
                    cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter("", false));

                    // Configure retry and error handling
                    cfg.UseMessageRetry(r =>
                    {
                        r.Exponential(5, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(5));
                        r.Ignore<ArgumentException>();
                        r.Ignore<InvalidOperationException>();
                    });

                    // Configure circuit breaker
                    cfg.UseCircuitBreaker(cb =>
                    {
                        cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                        cb.TripThreshold = 15;
                        cb.ActiveThreshold = 10;
                        cb.ResetInterval = TimeSpan.FromMinutes(5);
                    });

                    // Configure rate limiting
                    cfg.UseRateLimit(1000, TimeSpan.FromMinutes(1));
                });
            });

            // Register event bus
            services.AddScoped<IEventBus, EventBus>();
            services.AddScoped<IEventStore, InMemoryEventStore>();
            
            return services;
        }

        private static void ConfigureMessageTopology(IRabbitMqBusFactoryConfigurator cfg)
        {
            // Configure exchanges and routing for different event types
            
            // Order events exchange
            cfg.Message<OrderCreatedEvent>(m => m.SetEntityName("orders.created"));
            cfg.Message<OrderStatusChangedEvent>(m => m.SetEntityName("orders.status-changed"));
            cfg.Message<OrderCancelledEvent>(m => m.SetEntityName("orders.cancelled"));
            
            // Product events exchange
            cfg.Message<ProductCreatedEvent>(m => m.SetEntityName("products.created"));
            cfg.Message<ProductUpdatedEvent>(m => m.SetEntityName("products.updated"));
            cfg.Message<StockUpdatedEvent>(m => m.SetEntityName("products.stock-updated"));
            
            // Payment events exchange
            cfg.Message<PaymentProcessedEvent>(m => m.SetEntityName("payments.processed"));
            
            // Inventory events exchange
            cfg.Message<InventoryReservedEvent>(m => m.SetEntityName("inventory.reserved"));
            
            // Customer events exchange
            cfg.Message<CustomerRegisteredEvent>(m => m.SetEntityName("customers.registered"));
            cfg.Message<UserLoginEvent>(m => m.SetEntityName("users.login"));
            cfg.Message<UserLogoutEvent>(m => m.SetEntityName("users.logout"));
            
            // Shipment events exchange
            cfg.Message<ShipmentCreatedEvent>(m => m.SetEntityName("shipments.created"));
            cfg.Message<ShipmentStatusChangedEvent>(m => m.SetEntityName("shipments.status-changed"));
            
            // Notification events exchange
            cfg.Message<NotificationRequestedEvent>(m => m.SetEntityName("notifications.requested"));
            cfg.Message<NotificationSentEvent>(m => m.SetEntityName("notifications.sent"));
            
            // Analytics events exchange
            cfg.Message<ProductViewedEvent>(m => m.SetEntityName("analytics.product-viewed"));
            cfg.Message<CartUpdatedEvent>(m => m.SetEntityName("analytics.cart-updated"));
            
            // System events exchange
            cfg.Message<SystemHealthCheckEvent>(m => m.SetEntityName("system.health-check"));
            cfg.Message<ServiceStartedEvent>(m => m.SetEntityName("system.service-started"));
            cfg.Message<ServiceStoppedEvent>(m => m.SetEntityName("system.service-stopped"));
        }
    }

    public class RabbitMqSettings
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string VirtualHost { get; set; } = "/";
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
    }
}

namespace BuildingBlocks.Messaging.EventDriven
{
    /// <summary>
    /// Event bus interface for publishing events
    /// </summary>
    public interface IEventBus
    {
        Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : BaseEvent;
        Task PublishAsync<T>(IEnumerable<T> events, CancellationToken cancellationToken = default) where T : BaseEvent;
        Task PublishDelayedAsync<T>(T @event, TimeSpan delay, CancellationToken cancellationToken = default) where T : BaseEvent;
    }

    /// <summary>
    /// Event bus implementation using MassTransit
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly IBus _bus;
        private readonly IEventStore _eventStore;

        public EventBus(IBus bus, IEventStore eventStore)
        {
            _bus = bus;
            _eventStore = eventStore;
        }

        public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : BaseEvent
        {
            // Store event for audit trail
            await _eventStore.SaveAsync(@event);

            // Publish event
            await _bus.Publish(@event, cancellationToken);
        }

        public async Task PublishAsync<T>(IEnumerable<T> events, CancellationToken cancellationToken = default) where T : BaseEvent
        {
            var eventList = events.ToList();
            
            // Store events for audit trail
            foreach (var @event in eventList)
            {
                await _eventStore.SaveAsync(@event);
            }

            // Publish events
            await _bus.PublishBatch(eventList, cancellationToken);
        }

        public async Task PublishDelayedAsync<T>(T @event, TimeSpan delay, CancellationToken cancellationToken = default) where T : BaseEvent
        {
            // Store event for audit trail
            await _eventStore.SaveAsync(@event);

            // For delayed publishing, we'll use a simple delay approach
            // In production, consider using Quartz.NET or similar scheduling library
            _ = Task.Run(async () =>
            {
                await Task.Delay(delay, cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    await _bus.Publish(@event, cancellationToken);
                }
            }, cancellationToken);
        }
    }

    /// <summary>
    /// Event store interface for persisting events
    /// </summary>
    public interface IEventStore
    {
        Task SaveAsync<T>(T @event) where T : BaseEvent;
        Task<IEnumerable<BaseEvent>> GetEventsAsync(Guid aggregateId);
        Task<IEnumerable<BaseEvent>> GetEventsAsync(string eventType, DateTime from, DateTime to);
    }

    /// <summary>
    /// Simple in-memory event store implementation
    /// </summary>
    public class InMemoryEventStore : IEventStore
    {
        private readonly List<BaseEvent> _events = new();
        private readonly object _lock = new object();

        public Task SaveAsync<T>(T @event) where T : BaseEvent
        {
            lock (_lock)
            {
                _events.Add(@event);
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<BaseEvent>> GetEventsAsync(Guid aggregateId)
        {
            lock (_lock)
            {
                var events = _events.Where(e => e.Id == aggregateId).ToList();
                return Task.FromResult<IEnumerable<BaseEvent>>(events);
            }
        }

        public Task<IEnumerable<BaseEvent>> GetEventsAsync(string eventType, DateTime from, DateTime to)
        {
            lock (_lock)
            {
                var events = _events
                    .Where(e => e.EventType == eventType && 
                               e.OccurredOn >= from && 
                               e.OccurredOn <= to)
                    .ToList();
                return Task.FromResult<IEnumerable<BaseEvent>>(events);
            }
        }
    }
}

namespace BuildingBlocks.Messaging.EventDriven
{
    /// <summary>
    /// Event sourcing patterns and utilities
    /// </summary>
    public abstract class AggregateRoot
    {
        private readonly List<BaseEvent> _domainEvents = new();

        public Guid Id { get; protected set; }
        public int Version { get; protected set; }

        public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(BaseEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        public void LoadFromHistory(IEnumerable<BaseEvent> events)
        {
            foreach (var @event in events)
            {
                ApplyEvent(@event, false);
                Version++;
            }
        }

        protected abstract void ApplyEvent(BaseEvent @event, bool isNew = true);
    }

    /// <summary>
    /// Event-driven saga coordinator
    /// </summary>
    public abstract class SagaOrchestrator<TData> : 
        ISaga<TData> where TData : class, ISagaData, new()
    {
        protected readonly IEventBus EventBus;

        protected SagaOrchestrator(IEventBus eventBus)
        {
            EventBus = eventBus;
        }

        public TData Data { get; set; } = new();

        protected async Task PublishEventAsync<T>(T @event) where T : BaseEvent
        {
            await EventBus.PublishAsync(@event);
        }

        protected async Task ScheduleEventAsync<T>(T @event, TimeSpan delay) where T : BaseEvent
        {
            await EventBus.PublishDelayedAsync(@event, delay);
        }

        public abstract Task HandleAsync<T>(ConsumeContext<T> context) where T : class;
    }

    public interface ISagaData
    {
        Guid CorrelationId { get; set; }
        string CurrentState { get; set; }
    }

    public interface ISaga<TData> where TData : ISagaData
    {
        TData Data { get; set; }
    }
}
