using BuildingBlocks.DependencyInjection;
using BuildingBlocks.Identity;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Configuration;
using BuildingBlocks.Messaging.MassTransit;
using BuildingBlocks.Messaging.Outbox;
using BuildingBlocks.Observability;
using BuildingBlocks.ServiceDiscovery;
using BuildingBlocks.ServiceMesh;
using Cart.API.Extensions;
using Cart.Application.Handlers.Commands;
using Cart.Infrastructure.BackgroundJobs;
using Cart.Infrastructure.Data;
using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// === 1. Add BuildingBlocks Foundational Services ===
builder.Services.AddHttpContextAccessor();
builder.Services.AddConsulServiceDiscovery(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddServiceMeshHttpClient(builder.Configuration);

// === 2. Add Infrastructure Services (Database, Redis, etc.) ===
builder.Services.AddDbContext<CartDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));


// === اضافه کردن Hangfire ===
// 1. ثبت سرویس‌های Hangfire
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("Database"))
    ));

// 2. ثبت سرور Hangfire برای پردازش Job ها
builder.Services.AddHangfireServer();

// 3. ثبت کلاس Job شما در DI Container
builder.Services.AddScoped<CartJobs>();


// [جدید] ثبت UnitOfWork برای CartDbContext
builder.Services.AddUnitOfWorkAndDbContext<CartDbContext>();

// [جدید] فعال‌سازی پردازشگر Outbox
builder.Services.AddOutboxMessageProcessor(builder.Configuration);

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
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
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


app.UseHangfireDashboard("/hangfire");
// این کار باعث می‌شود متد ProcessExpiredCartsAsync هر 5 دقیقه یکبار اجرا شود
RecurringJob.AddOrUpdate<CartJobs>(
    "process-expired-carts", // یک شناسه منحصر به فرد برای Job
    job => job.ProcessExpiredCartsAsync(null!, null!, null!), // پارامترها نادیده گرفته می‌شوند، Hangfire خودش آن‌ها را تزریق می‌کند
    Cron.MinuteInterval(5) // هر 5 دقیقه
);

app.MapControllers();


// Auto-migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CartDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();