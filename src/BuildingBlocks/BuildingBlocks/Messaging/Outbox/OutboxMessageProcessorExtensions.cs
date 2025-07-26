using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.Outbox
{
    /// <summary>
    /// Extension methods for configuring the transactional outbox pattern.
    /// </summary>
    public static class OutboxProcessorExtensions
    {
        /// <summary>
        /// Adds and configures the Outbox Message Processor background service.
        /// </summary>
        public static IServiceCollection AddOutboxMessageProcessor(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OutboxOptions>(configuration.GetSection("Outbox"));
            services.AddHostedService<OutboxMessageProcessor>();

            return services;
        }
    }
}