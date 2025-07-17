using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Messaging;
using BuildingBlocks.Identity;

namespace BuildingBlocks.Samples
{
    /// <summary>
    /// Sample Program.cs implementations for different types of microservices
    /// These are examples showing how to configure each building block
    /// </summary>
    public class SamplePrograms
    {
        /// <summary>
        /// Complete microservice setup with all building blocks
        /// This demonstrates the configuration patterns for a full microservice
        /// </summary>
        public static void ConfigureCompleteMicroservice(WebApplicationBuilder builder)
        {
            // 1. Logging with Serilog
            builder.Host.UseSerilog((context, config) =>
            {
                config.ReadFrom.Configuration(context.Configuration)
                      .Enrich.WithProperty("Service", context.HostingEnvironment.ApplicationName)
                      .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                      .WriteTo.Console()
                      .WriteTo.Seq("http://localhost:5341");
            });

            // 2. Service Discovery with Consul (would use extension methods from ConsulServiceDiscovery.cs)
            // builder.Services.AddConsulServiceDiscovery(builder.Configuration);

            // 3. Messaging with MassTransit and RabbitMQ
            builder.Services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            // Register message bus
            builder.Services.AddScoped<IMessageBus, MessageBus>();

            // 4. Authentication with JWT
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = builder.Configuration["JWT:Authority"];
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });

            // 5. Health Checks
            builder.Services.AddHealthChecks()
                .AddConsul(options =>
                {
                    options.HostName = "localhost";
                    options.Port = 8500;
                });

            // Standard ASP.NET Core services
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            // Configure middleware pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHealthChecks("/health");
            app.MapControllers();

            // Map sample endpoints
            app.MapSampleEndpoints();

            app.Run();
        }

        /// <summary>
        /// Identity Server setup with IdentityServer 8 - demonstrates centralized authentication
        /// </summary>
        public static void ConfigureIdentityServer8(WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, config) =>
            {
                config.ReadFrom.Configuration(context.Configuration)
                      .WriteTo.Console()
                      .WriteTo.Seq("http://localhost:5341");
            });

            // Identity Server 8 configuration
            builder.Services.AddIdentityServer8(builder.Configuration);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer8();

            app.Run();
        }
    }

    /// <summary>
    /// Configuration models for different services
    /// </summary>
    public class RabbitMqSettings
    {
        public string Host { get; set; } = "localhost";
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public int Port { get; set; } = 5672;
    }

    public class ConsulSettings
    {
        public string Address { get; set; } = "http://localhost:8500";
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceId { get; set; } = string.Empty;
        public int Port { get; set; }
        public string[] Tags { get; set; } = Array.Empty<string>();
    }

    /// <summary>
    /// Sample minimal API endpoints demonstrating building blocks usage
    /// </summary>
    public static class SampleEndpoints
    {
        public static void MapSampleEndpoints(this WebApplication app)
        {
            var sampleGroup = app.MapGroup("/api/samples")
                .RequireAuthorization();

            // Publish event endpoint
            sampleGroup.MapPost("/events/publish", async (
                IPublishEndpoint publishEndpoint,
                PublishEventRequest request) =>
            {
                var eventToPublish = new OrderCreatedEvent
                {
                    OrderId = int.Parse(request.OrderId),
                    Amount = request.TotalAmount,
                    CustomerEmail = request.UserId + "@example.com",
                    OrderDate = DateTime.UtcNow
                };

                await publishEndpoint.Publish(eventToPublish);
                return Microsoft.AspNetCore.Http.Results.Ok(new { Message = "Event published successfully" });
            });

            // Health check endpoint
            sampleGroup.MapGet("/health", () =>
            {
                return Microsoft.AspNetCore.Http.Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
            });
        }
    }

    public record PublishEventRequest(
        string OrderId,
        string UserId,
        string ProductName,
        int Quantity,
        decimal TotalAmount);
}
