using MassTransit;
using Microsoft.OpenApi.Models;
using Serilog.Formatting.Compact;
using Serilog;
using logger = Serilog.ILogger;
using OrderService.Application;
using OrderService.Infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

// Program.cs: Main entry point for the Order Service API using .NET minimal hosting model
// --------------------------------------------------------------

// Build the WebApplication host and configure services
var builder = WebApplication.CreateBuilder(args);

// Configure a global Serilog logger
var logger = ConfigureLogger();
logger.Information("Logger configured");



// Register application-specific services
builder.Services.AddApplicationServices(builder.Configuration);

//  Register infrastructure services
var databaseSettings = builder.Configuration.GetSection("DatabaseSettings");
builder.Services.AddInfrastructureServices(databaseSettings);

// Add JSON support for controllers via Newtonsoft
builder.Services.AddControllers().AddNewtonsoftJson();

// Enable Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order Service API", Version = "v1" });
});

// Configure MassTransit with RabbitMQ as the transport
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

var app = builder.Build();

// Enable CloudEvents and subscription handler for event-driven communication
//app.UseCloudEvents();
//app.MapSubscribeHandler();

// In development, use Swagger UI; otherwise configure error pages and HTTPS
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

// Set up routing
app.UseRouting();

// Use health checks and authorization middlewares
app.UseHealthChecks("/health");
app.UseAuthorization();

// Map controller endpoints
app.MapControllers();

// Run the app
app.Run();

// Helper method to configure Serilog with console and rolling file outputs
static logger ConfigureLogger()
{
    return new LoggerConfiguration()
        .Enrich.FromLogContext()
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Context}] {Message:lj}{NewLine}{Exception}"
        )
        .WriteTo.RollingFile(new CompactJsonFormatter(), "logs/logs")
        .CreateLogger();
}
