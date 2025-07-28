using BuildingBlocks.ApiGateway; // برای Yarp اگر Gateway هم در این پروژه باشد
using BuildingBlocks.Configuration;
using BuildingBlocks.DependencyInjection;
using BuildingBlocks.HealthCheck;
using BuildingBlocks.Messaging.Configuration;
using BuildingBlocks.Messaging.Outbox;
using BuildingBlocks.Observability;
using BuildingBlocks.Resiliency;
using BuildingBlocks.ServiceDiscovery;
using BuildingBlocks.ServiceMesh;
using ProductService.Application; // برای AddApplicationServices
using ProductService.Infrastructure; // برای AddInfrastructureServices

var builder = WebApplication.CreateBuilder(args);

// 1. Centralized Configuration
builder.Configuration.AddConsulConfiguration(builder.Configuration);

// 2. Logging and Observability
builder.Host.AddSerilogLogging();
builder.Services.AddObservability(builder.Configuration);

// 3. Add BuildingBlocks and Core Services
//builder.Services.AddSharedKernel(builder.Configuration, typeof(ProductService.Application.AssemblyReference).Assembly);

// 4. Add Infrastructure Layer (DbContext, Repositories, etc.)
builder.Services.AddInfrastructureServices(builder.Configuration);

// 5. Add Application Layer (CQRS Handlers, Mappers, etc.)
builder.Services.AddApplicationServices();

// 6. Service Discovery and Mesh
builder.Services.AddConsulServiceDiscovery(builder.Configuration);
builder.Services.AddServiceMeshHttpClient(builder.Configuration);

// 7. Event-Driven Messaging with MassTransit and Outbox
//builder.Services.AddEventDrivenMessaging(builder.Configuration, typeof(ProductService.Application.AssemblyReference).Assembly);
builder.Services.AddOutboxMessageProcessor(builder.Configuration);

// 8. Resiliency Policies
builder.Services.AddResiliency(builder.Configuration);

// 9. API Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Product Service API", Version = "v1" });
});

// 10. Health Checks
builder.Services.AddBuildingBlocksHealthChecks(builder.Configuration);

// 11. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        // در پروداکشن آدرس‌های مشخص را جایگزین کنید
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // Seed database in development
    await app.InitialiseDatabaseAsync();
}

app.UseHttpsRedirection();
app.UseCors("DefaultCorsPolicy");

// [مهم] اضافه کردن Middleware های BuildingBlocks
app.UseObservability();
app.UseBuildingBlocksHealthChecks();

app.UseAuthorization();
app.MapControllers();

app.Run();

