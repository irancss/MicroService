using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Consumers;
using NotificationService.Infrastructure.Providers;
using NotificationService.Infrastructure.Repositories;
using NotificationService.Infrastructure.Services;

namespace NotificationService.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // MongoDB
        services.AddSingleton<IMongoClient>(sp =>
        {
            var connectionString = configuration.GetConnectionString("MongoDB")!;
            return new MongoClient(connectionString);
        });

        services.AddScoped<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var databaseName = configuration["MongoDB:DatabaseName"]!;
            return client.GetDatabase(databaseName);
        });

        // Repositories
        services.AddScoped<INotificationTemplateRepository, MongoNotificationTemplateRepository>();
        services.AddScoped<INotificationLogRepository, MongoNotificationLogRepository>();

        // Providers
        services.Configure<SendGridOptions>(configuration.GetSection("SendGrid"));
        services.Configure<KavenegarOptions>(configuration.GetSection("Kavenegar"));

        //services.AddSendGrid(options =>
        //{
        //    options.ApiKey = configuration["SendGrid:ApiKey"]!;
        //});

        services.AddHttpClient<KavenegarSmsProvider>();
        
        services.AddScoped<IEmailProvider, SendGridEmailProvider>();
        services.AddScoped<ISmsProvider, KavenegarSmsProvider>();

        // Template Engine
        services.AddScoped<ITemplateEngine, ScribanTemplateEngine>();

        // MassTransit
        services.AddMassTransit(x =>
        {
            x.AddConsumer<UserRegisteredConsumer>();
            x.AddConsumer<OrderPlacedConsumer>();
            x.AddConsumer<PasswordResetRequestedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqSettings = configuration.GetSection("RabbitMQ");
                cfg.Host(rabbitMqSettings["Host"], h =>
                {
                    h.Username(rabbitMqSettings["Username"]!);
                    h.Password(rabbitMqSettings["Password"]!);
                });

                cfg.ReceiveEndpoint("notification-user-registered", e =>
                {
                    e.ConfigureConsumer<UserRegisteredConsumer>(context);
                });

                cfg.ReceiveEndpoint("notification-order-placed", e =>
                {
                    e.ConfigureConsumer<OrderPlacedConsumer>(context);
                });

                cfg.ReceiveEndpoint("notification-password-reset", e =>
                {
                    e.ConfigureConsumer<PasswordResetRequestedConsumer>(context);
                });
            });
        });

        return services;
    }
}
