using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using Shared.Kernel.Behaviors;
using Shared.Kernel.Middleware;
using ShippingService.Application.Mappings;
using ShippingService.Application.Services;
using ShippingService.Application.Validators;
using ShippingService.Domain.Repositories;
using ShippingService.Domain.Services;
using ShippingService.Infrastructure.Data;
using ShippingService.Infrastructure.Repositories;
using ShippingService.Infrastructure.Services;
using ShippingService.API.Authentication;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. Database Configuration
builder.Services.AddDbContext<ShippingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("ShippingService.Infrastructure")));

// 2. MediatR Configuration
var apiAssembly = Assembly.GetExecutingAssembly();
var applicationAssembly = typeof(ShippingService.Application.Commands.CreateShippingMethodCommand).Assembly;
var infrastructureAssembly = typeof(ShippingService.Infrastructure.Data.ShippingDbContext).Assembly;

Console.WriteLine($"Registering MediatR from assemblies:");
Console.WriteLine($"  - API: {apiAssembly.FullName}");
Console.WriteLine($"  - Application: {applicationAssembly.FullName}");
Console.WriteLine($"  - Infrastructure: {infrastructureAssembly.FullName}");

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(apiAssembly);
    cfg.RegisterServicesFromAssembly(applicationAssembly);
    cfg.RegisterServicesFromAssembly(infrastructureAssembly);
});

// 3. Add Pipeline Behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// 4. FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateShippingMethodCommandValidator>();

// 5. AutoMapper
builder.Services.AddAutoMapper(typeof(ShippingMappingProfile));

// 6. Repository Registration
builder.Services.AddScoped<IShippingMethodRepository, ShippingMethodRepository>();
builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();
builder.Services.AddScoped<IShipmentReturnRepository, ShipmentReturnRepository>();
builder.Services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();

// 7. Domain Service Registration
builder.Services.AddScoped<IRouteOptimizationService, GoogleRouteOptimizationService>();
builder.Services.AddScoped<INotificationService, TwilioNotificationService>();
builder.Services.AddScoped<ShippingService.Domain.Services.IFreeShippingRuleService, ShippingService.Infrastructure.Services.FreeShippingRuleService>();
builder.Services.AddScoped<ShippingService.Domain.Services.IPremiumSubscriptionService, ShippingService.Infrastructure.Services.PremiumSubscriptionService>();
builder.Services.AddScoped<IShippingCostCalculatorService, ShippingCostCalculatorService>();

// 8. Application Service Registration  
builder.Services.AddScoped<ShippingService.Application.Services.IFreeShippingRuleService, ApplicationFreeShippingRuleService>();

// 8. HTTP Clients with Polly
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttempt =>
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

builder.Services.AddHttpClient<IProductServiceClient, ProductServiceClient>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductApi"] ?? "https://localhost:7001");
        client.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);

// 9. Authorization Policies (برای حالت Development - در Production با Auth microservice کانفیگ کنید)
// نکته: در حالت Production این قسمت را با Auth microservice یکپارچه کنید
if (builder.Environment.IsDevelopment())
{
    // Development mode - اجازه دسترسی بدون authentication
    builder.Services.AddAuthentication("Bearer")
        .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, DevelopmentAuthenticationHandler>("Bearer", options => { });
}
else
{
    // Production mode - JWT authentication
    builder.Services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
            options.Authority = builder.Configuration["Auth:Authority"]; // Auth microservice URL
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateAudience = false
            };
        });
}

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});

// 10. API Configuration
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true; // Let FluentValidation handle validation
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "Shipping Service API", 
        Version = "v1",
        Description = "A comprehensive shipping service with CQRS architecture"
    });
    
    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// 11. Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.

// 1. Exception Handling Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 2. Development Configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shipping Service API v1");
    });
}

// 3. Standard Pipeline
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// 4. Health Checks
app.MapHealthChecks("/health");

// 5. Controllers
app.MapControllers();

// 6. Database Migration (Optional - for development)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ShippingDbContext>();
    await context.Database.EnsureCreatedAsync();
}

app.Run();
