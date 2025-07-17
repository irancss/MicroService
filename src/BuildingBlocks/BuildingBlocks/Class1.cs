using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using BuildingBlocks.Messaging.Configuration;
using BuildingBlocks.Messaging.EventDriven;
using BuildingBlocks.Messaging.Handlers;
using BuildingBlocks.ApiGateway.Configuration;
using BuildingBlocks.ServiceMesh.Configuration;
using BuildingBlocks.HealthCheck;

namespace BuildingBlocks
{
    /// <summary>
    /// Extension methods to easily configure all BuildingBlocks components
    /// </summary>
    public static class BuildingBlocksExtensions
    {
        /// <summary>
        /// Add all BuildingBlocks services to the dependency injection container
        /// </summary>
        public static IServiceCollection AddBuildingBlocks(this IServiceCollection services, IConfiguration configuration)
        {
            // Add MassTransit with RabbitMQ
            services.AddMassTransitWithRabbitMq(configuration, cfg =>
            {
                // Add sample event handlers
                cfg.AddConsumer<OrderCreatedEventHandler>();
                cfg.AddConsumer<PaymentProcessedEventHandler>();
                cfg.AddConsumer<InventoryReservedEventHandler>();
                
                // Add domain event handlers
                cfg.AddConsumer<ProductEventHandler>();
                cfg.AddConsumer<OrderEventHandler>();
                cfg.AddConsumer<CustomerEventHandler>();
                cfg.AddConsumer<ShipmentEventHandler>();
                cfg.AddConsumer<NotificationEventHandler>();
                cfg.AddConsumer<AnalyticsEventHandler>();
            });

            // Add Service Mesh (Consul, Service Discovery, Load Balancing, Circuit Breaker)
            services.AddServiceMesh(configuration);

            // Add API Gateway (Ocelot)
            services.AddApiGateway(configuration);

            // Add Health Checks
            services.AddBuildingBlocksHealthChecks(configuration);

            return services;
        }

        /// <summary>
        /// Add BuildingBlocks with Event-Driven Architecture capabilities
        /// </summary>
        public static IServiceCollection AddBuildingBlocksWithEventDriven(
            this IServiceCollection services, 
            IConfiguration configuration,
            params Type[] consumerAssemblyMarkers)
        {
            // Add base BuildingBlocks
            services.AddBuildingBlocks(configuration);

            // Add Event-Driven Architecture with automatic consumer discovery
            services.AddEventDrivenArchitecture(configuration, consumerAssemblyMarkers);

            return services;
        }

        /// <summary>
        /// Configure the application to use BuildingBlocks components
        /// </summary>
        public static IApplicationBuilder UseBuildingBlocks(this IApplicationBuilder app)
        {
            // Use API Gateway middleware
            app.UseApiGateway();

            // Use Health Checks
            app.UseBuildingBlocksHealthChecks();

            return app;
        }
    }

    /// <summary>
    /// Sample startup configuration for a microservice using BuildingBlocks
    /// </summary>
    public class SampleStartup
    {
        private readonly IConfiguration _configuration;

        public SampleStartup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add BuildingBlocks with Event-Driven Architecture
            services.AddBuildingBlocksWithEventDriven(_configuration, typeof(OrderCreatedEventHandler));

            // Add your application-specific services
            // services.AddScoped<IYourService, YourService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Use BuildingBlocks
            app.UseBuildingBlocks();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    /// <summary>
    /// Sample event-driven startup for modern microservices
    /// </summary>
    public class EventDrivenStartup
    {
        private readonly IConfiguration _configuration;

        public EventDrivenStartup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add controllers
            services.AddControllers();

            // Add BuildingBlocks with full Event-Driven capabilities
            services.AddBuildingBlocksWithEventDriven(_configuration, 
                typeof(OrderCreatedEventHandler),
                typeof(ProductEventHandler),
                typeof(CustomerEventHandler));

            // Add application-specific services
            // services.AddScoped<IOrderService, OrderService>();
            // services.AddScoped<IProductService, ProductService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Use BuildingBlocks middleware
            app.UseBuildingBlocks();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
