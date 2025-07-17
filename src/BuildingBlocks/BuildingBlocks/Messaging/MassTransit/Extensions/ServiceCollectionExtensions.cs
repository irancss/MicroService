using BuildingBlocks.Messaging.Abstractions;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.MassTransit.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(
            this IServiceCollection services, 
            IConfiguration configuration,
            string serviceName,
            Action<IBusRegistrationConfigurator>? configureConsumers = null)
        {
            services.AddMassTransit(x =>
            {
                // Configure consumers
                configureConsumers?.Invoke(x);

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMqSettings = configuration.GetSection("RabbitMQ");
                    var host = rabbitMqSettings["Host"] ?? "localhost";
                    var port = rabbitMqSettings.GetValue<int>("Port", 5672);
                    var username = rabbitMqSettings["Username"] ?? "guest";
                    var password = rabbitMqSettings["Password"] ?? "guest";

                    cfg.Host(host, port, "/", h =>
                    {
                        h.Username(username);
                        h.Password(password);
                    });

                    // Set queue name format
                    cfg.ReceiveEndpoint($"{serviceName.ToLower()}-queue", e =>
                    {
                        e.ConfigureConsumers(context);
                    });

                    // Configure message topology
                    cfg.ConfigureEndpoints(context);
                    
                    // Set message retry policy
                    cfg.UseMessageRetry(r => r.Immediate(3));
                });
            });

            services.AddScoped<IEventBus, MassTransitEventBus>();
            
            return services;
        }
    }
}