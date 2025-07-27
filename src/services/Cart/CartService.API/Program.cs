using BuildingBlocks.DependencyInjection;
using BuildingBlocks.Identity;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Configuration;
using BuildingBlocks.Messaging.MassTransit;
using BuildingBlocks.Observability;
using BuildingBlocks.ServiceDiscovery;
using BuildingBlocks.ServiceMesh;
using Cart.API.Extensions;
using Cart.Application.Handlers.Commands;
using Cart.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// === 1. Add BuildingBlocks Foundational Services ===
builder.Services.AddHttpContextAccessor();
builder.Services.AddConsulServiceDiscovery(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddServiceMeshHttpClient(builder.Configuration);

// === 2. Add Infrastructure Services (Database, Redis, etc.) ===
builder.Services.AddDbContext<CartDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddInfrastructureServices(builder.Configuration);

// === 3. Add MediatR & CQRS pipeline behaviors from BuildingBlocks ===
builder.Services.AddSharedKernel(builder.Configuration, typeof(AddItemToActiveCartHandler).Assembly);

// === 4. Add Messaging (MassTransit & RabbitMQ) ===
builder.Services.AddMassTransit(config =>
{
    // config.AddConsumer<...>(); // Register consumers here
    config.UsingRabbitMq((context, cfg) =>
    {
        var settings = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>() ?? new RabbitMqSettings();
        cfg.Host(settings.Host, settings.VirtualHost, h =>
        {
            h.Username(settings.Username);
            h.Password(settings.Password);
        });
        cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter("cart-service", false));
    });
});
builder.Services.AddScoped<IEventBus, MassTransitEventBus>();
builder.Services.AddScoped<IMessageBus, MessageBus>();


// === 5. Add Observability ===
builder.Host.AddSerilogLogging();
builder.Services.AddObservability(builder.Configuration);

// === 6. Standard API Services ===
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Cart.API", Version = "v1" });
    // TODO: Add JWT Security Definition for Swagger
});

// ================= Build The App ===================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseObservability();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Auto-migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CartDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();