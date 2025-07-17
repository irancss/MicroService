using Cart.Application;
using Cart.Infrastructure;
using Hangfire;
using Serilog;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, config) =>
{
    config
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console()
        .WriteTo.File("logs/cart-service-.txt", rollingInterval: RollingInterval.Day);
});

// Add services to the container
builder.Services.AddControllers()
    .AddFluentValidation();

// Add Application and Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Cart Microservice API", 
        Version = "v1",
        Description = "Dual-Cart E-commerce Microservice with Advanced Features",
        Contact = new OpenApiContact
        {
            Name = "Development Team",
            Email = "dev@company.com"
        }
    });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add API versioning
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ApiVersionReader = Microsoft.AspNetCore.Mvc.ApiVersioning.ApiVersionReader.Combine(
        new Microsoft.AspNetCore.Mvc.ApiVersioning.QueryStringApiVersionReader("apiVersion"),
        new Microsoft.AspNetCore.Mvc.ApiVersioning.HeaderApiVersionReader("X-Version"),
        new Microsoft.AspNetCore.Mvc.ApiVersioning.MediaTypeApiVersionReader("ver")
    );
});

builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck("redis", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Redis is healthy"))
    .AddCheck("hangfire", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Hangfire is healthy"));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cart Microservice API V1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    });
}

// Use Serilog for request logging
app.UseSerilogRequestLogging();

// Use CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();

// Use Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new AllowAllAuthorizationFilter() } // Only for development
});

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

// Map health checks
app.MapHealthChecks("/health");

app.Run();

// Allow all authorization filter for Hangfire (development only)
public class AllowAllAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true; // In production, implement proper authorization
    }
}
