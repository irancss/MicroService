using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Messaging.Configuration
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(
            this IServiceCollection services, 
            IConfiguration configuration,
            Action<IBusRegistrationConfigurator>? configure = null)
        {
            var rabbitMqSettings = configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>();
            
            if (rabbitMqSettings == null)
            {
                throw new InvalidOperationException("RabbitMQ configuration is missing");
            }

            services.AddMassTransit(x =>
            {
                // Add consumers from the calling assembly
                configure?.Invoke(x);

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
                    {
                        h.Username(rabbitMqSettings.Username);
                        h.Password(rabbitMqSettings.Password);
                    });

                    cfg.ConfigureEndpoints(context);

                    // Configure retry policy
                    cfg.UseMessageRetry(r =>
                    {
                        r.Interval(3, TimeSpan.FromSeconds(5));
                        r.Ignore<ArgumentException>();
                    });

                    // Configure circuit breaker
                    cfg.UseCircuitBreaker(cb =>
                    {
                        cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                        cb.TripThreshold = 15;
                        cb.ActiveThreshold = 10;
                        cb.ResetInterval = TimeSpan.FromMinutes(5);
                    });
                });
            });

            services.AddScoped<IMessageBus, MessageBus>();

            return services;
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
