using System.Reflection;
using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.EventDriven.Sagas;
using BuildingBlocks.Messaging.MassTransit;
using BuildingBlocks.Messaging.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.Configuration
{
    /// <summary>
    /// [اصلاح شد] Configures a comprehensive event-driven messaging setup with MassTransit.
    /// </summary>
    public static class MessagingExtensions
    {
        public static IServiceCollection AddEventDrivenMessaging(
            this IServiceCollection services,
            IConfiguration configuration,
            params Assembly[] assembliesToScan)
        {
            if (!assembliesToScan.Any())
            {
                assembliesToScan = new[] { Assembly.GetCallingAssembly() };
            }

            // 1. Register Saga DbContext
            services.AddDbContext<SagaDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("SagaDatabase");
                if (string.IsNullOrEmpty(connectionString))
                    throw new InvalidOperationException("Connection string 'SagaDatabase' not found.");

                options.UseNpgsql(connectionString, m =>
                {
                    m.MigrationsAssembly(typeof(SagaDbContext).Assembly.FullName);
                });
            });

            // 2. Configure MassTransit
            services.AddMassTransit(x =>
            {
                // Auto-discover and register all Consumers and Saga State Machines
                x.AddConsumers(assembliesToScan);
                x.AddSagaStateMachines(assembliesToScan);

                // Configure EF Core for Saga persistence
                x.SetEntityFrameworkSagaRepositoryProvider(r =>
                {
                    r.ExistingDbContext<SagaDbContext>();
                    r.UsePostgres();
                });

                // Configure RabbitMQ as the transport
                x.UsingRabbitMq((context, cfg) =>
                {
                    var settings = configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>() ?? new RabbitMqSettings();

                    cfg.Host(settings.Host, settings.VirtualHost, h =>
                    {
                        h.Username(settings.Username);
                        h.Password(settings.Password);
                    });

                    // Automatically configure endpoints for all registered consumers and sagas
                    cfg.UseInMemoryOutbox();
                    cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(prefix: configuration["Service:Name"], includeNamespace: false));


                    // [توضیح] این سیاست Retry به طور خودکار پیام را پس از 3 بار تلاش ناموفق
                    // به صف `_error` منتقل می‌کند. این همان Dead-lettering است.
                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                    cfg.UseCircuitBreaker(cb =>
                    {
                        cb.TripThreshold = 15;
                        cb.ActiveThreshold = 10;
                        cb.ResetInterval = TimeSpan.FromMinutes(5);
                    });
                });
            });

            // 3. Register our custom abstractions
            services.AddScoped<IEventBus, MassTransitEventBus>();
            services.AddScoped<IMessageBus, MessageBus>(); // [جدید] ثبت MessageBus برای ارسال Commands
            services.AddScoped<IEventStore, EfEventStore>();

            return services;
        }
    }

    public class RabbitMqSettings
    {
        public string Host { get; set; } = "localhost";
        public string VirtualHost { get; set; } = "/";
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
    }
}