using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SearchService.Application.Services;
using SearchService.Domain.Interfaces;
using SearchService.Infrastructure.Consumers;
using SearchService.Infrastructure.Repositories;
using SearchService.Infrastructure.Services;

namespace SearchService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Elasticsearch
        services.AddElasticsearch(configuration);

        // Add repositories
        services.AddScoped<ISearchRepository, ElasticsearchRepository>();

        // Add services
        services.AddScoped<IElasticsearchQueryService, ElasticsearchQueryService>();
        
        // Add HTTP client for user personalization service
        services.AddHttpClient<IUserPersonalizationService, UserPersonalizationService>(client =>
        {
            var baseUrl = configuration.GetValue<string>("UserPersonalizationService:BaseUrl") 
                ?? "https://localhost:7001"; // Default fallback
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        // Add MassTransit for RabbitMQ
        services.AddMassTransit(x =>
        {
            // Add consumers
            x.AddConsumer<ProductCreatedEventConsumer>();
            x.AddConsumer<ProductUpdatedEventConsumer>();
            x.AddConsumer<ProductDeletedEventConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var connectionString = configuration.GetConnectionString("RabbitMQ") 
                    ?? "amqp://guest:guest@localhost:5672";
                cfg.Host(connectionString);

                // Configure endpoints
                cfg.ReceiveEndpoint("search-product-created", e =>
                {
                    e.ConfigureConsumer<ProductCreatedEventConsumer>(context);
                });

                cfg.ReceiveEndpoint("search-product-updated", e =>
                {
                    e.ConfigureConsumer<ProductUpdatedEventConsumer>(context);
                });

                cfg.ReceiveEndpoint("search-product-deleted", e =>
                {
                    e.ConfigureConsumer<ProductDeletedEventConsumer>(context);
                });
            });
        });

        return services;
    }

    private static IServiceCollection AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Elasticsearch") 
            ?? "http://localhost:9200";

        services.AddSingleton<ElasticsearchClient>(serviceProvider =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<ElasticsearchClient>>();

            var settings = new ElasticsearchClientSettings(new Uri(connectionString))
                .DisableDirectStreaming()
                .ServerCertificateValidationCallback((sender, certificate, chain, errors) => true) // For dev only
                .OnRequestCompleted(details =>
                {
                    if (details.HasSuccessfulStatusCode)
                    {
                        logger.LogDebug("Elasticsearch request to {Uri} completed successfully",
                            details.Uri);
                    }
                    else
                    {
                        logger.LogError("Elasticsearch request to {Uri} failed with status {StatusCode}",
                            details.Uri, details.HttpStatusCode);
                    }
                });

            // Add authentication if configured
            var username = configuration.GetValue<string>("Elasticsearch:Username");
            var password = configuration.GetValue<string>("Elasticsearch:Password");
            
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                settings.Authentication(new Elastic.Transport.BasicAuthentication(username, password));
            }

            return new ElasticsearchClient(settings);
        });

        return services;
    }
}
