using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProductApi.Infrastructure.Mapping;
using ProductApi.Infrastructure.Settings;
using ProductApi.Repositories;
using ProductApi.Services;
using ProductApi.Services.Caching;
using ProductApi.Validators;
using StackExchange.Redis;
using Polly; // برای Polly Resilience Policies
using Polly.Extensions.Http; // برای集成 Polly با HttpClientFactory
using FluentValidation; // برای FluentValidation
using FluentValidation.AspNetCore;
using MongoDB.Bson;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace; // برای集成 با ASP.NET Core
// برای OpenTelemetry Tracing

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

// 1. --- پیکربندی Settings ---
builder.Services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
// حذف MinioSettings
// حذف KafkaSettings
builder.Services.Configure<CdnSettings>(configuration.GetSection("CdnSettings"));
builder.Services.Configure<RedisSettings>(configuration.GetSection("RedisSettings")); // افزودن تنظیمات Redis

// 2. --- پیکربندی MongoDB ---
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var logger = sp.GetRequiredService<ILogger<Program>>();

    logger.LogInformation("🔹 Attempting MongoDB connection to: {ConnectionString}",
        settings.ConnectionString.Replace("mongodb://", "mongodb://[REDACTED]@")); // برای امنیت

    try
    {
        var client = new MongoClient(settings.ConnectionString);

        // تست اتصال
        var database = client.GetDatabase(settings.DatabaseName);
        database.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait();

        logger.LogInformation("✅ MongoDB connection established successfully!");
        return client;
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Failed to connect to MongoDB");
        throw; // خطا را به بالا پاس می‌دهد
    }
});
builder.Services.AddScoped<IProductRepository, MongoProductRepository>();


// 5. --- پیکربندی Redis ---
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisSettings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
    var configurationOptions = ConfigurationOptions.Parse(redisSettings.ConnectionString);
    configurationOptions.AbortOnConnectFail = redisSettings.AbortOnConnectFail;
    return ConnectionMultiplexer.Connect(configurationOptions);
});
builder.Services.AddSingleton<ICachingService, RedisCachingService>();

// 6. --- پیکربندی AutoMapper ---
builder.Services.AddAutoMapper(typeof(MappingProfile));

// 7. --- پیکربندی FluentValidation ---
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();

// 8. --- پیکربندی HttpClient برای سرویس‌های دیگر با Polly ---
// <summary>
// Defines a retry policy for HTTP requests that handles transient errors and 404 Not Found responses.
// The policy will retry the HTTP request up to three times, using an exponential backoff strategy
// (delays of 2^retryAttempt seconds between retries). On each retry, a warning is logged indicating
// the delay duration and the retry attempt number.
// the delay duration and the retry attempt number.
// </summary>
// <remarks>
// This policy is useful for improving the resilience of HTTP client operations by automatically retrying
// failed requests due to transient network issues or when a resource is temporarily unavailable (404 Not Found).
// The exponential backoff helps to reduce the load on the server and network during repeated failures.
// </remarks>
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        onRetry: (outcome, timespan, retryAttempt, context) => {
            // Logging is not available here by default; consider logging inside the handler or via middleware if needed.
        });

var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 5,
        durationOfBreak: TimeSpan.FromSeconds(30)
    );


builder.Services.AddHttpClient<IDiscountServiceClient, DiscountServiceClient>(client =>
    {
        client.BaseAddress = new Uri(configuration["ServiceUrls:DiscountApi"] ?? throw new ArgumentNullException("ServiceUrls:DiscountApi not configured"));
        client.Timeout = TimeSpan.FromSeconds(10);
    })
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);

// 9. --- ثبت سرویس‌ها ---
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IMediaService, MediaService>();

// 10. --- پیکربندی OpenTelemetry ---
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName: builder.Environment.ApplicationName))
    .WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
    );

// 7. --- سایر سرویس‌های ASP.NET Core ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Product Service API (Refactored)", Version = "v1" });
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    // افزودن تنظیمات امنیتی JWT به Swagger (اگر از Authorize استفاده می‌کنید)
    // options.AddSecurityDefinition(...)
    // options.AddSecurityRequirement(...)
});

builder.Services.AddHealthChecks();
    


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Service API v1"));
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
