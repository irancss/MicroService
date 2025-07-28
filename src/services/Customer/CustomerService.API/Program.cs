using BuildingBlocks.DependencyInjection;
using BuildingBlocks.Identity;
using BuildingBlocks.Messaging.Configuration;
using BuildingBlocks.Messaging.Outbox;
using BuildingBlocks.ServiceDiscovery;
using CustomerService.Domain.Interfaces;
using CustomerService.Infrastructure.Data;
using CustomerService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using BuildingBlocks.Infrastructure.Interceptors;
using CustomerService.API.Middlewares;
using BuildingBlocks.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
var assemblies = new[] { Assembly.GetExecutingAssembly() };

// 1. Add Building Blocks Services
builder.Services.AddSharedKernel(builder.Configuration, assemblies);
builder.Services.AddConsulServiceDiscovery(builder.Configuration);
builder.Services.AddEventDrivenMessaging(builder.Configuration, assemblies);
builder.Services.AddOutboxMessageProcessor(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

// 2. Add Database Context and Unit of Work
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// تزریق Interceptor ها به DbContext
builder.Services.AddDbContext<CustomerDbContext>((sp, options) =>
{
    options.UseNpgsql(connectionString)
           .AddInterceptors(sp.GetRequiredService<DomainEventDispatchingInterceptor>())
           .AddInterceptors(sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>());
});

// این ثبت‌ها تضمین می‌کنند که IApplicationDbContext و IUnitOfWork به درستی resolve شوند
builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<CustomerDbContext>());
builder.Services.AddUnitOfWorkAndDbContext<CustomerDbContext>();

// 3. Register Application-specific services
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(assemblies));

// 4. Add Web API Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Customer API (Production)", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Auto-migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();