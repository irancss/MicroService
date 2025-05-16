using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProductApi.Infrastructure.Kafka;
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
using FluentValidation.AspNetCore; // برای集成 با ASP.NET Core
// برای OpenTelemetry Tracing

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


// Add services to the container.

// 1. --- پیکربندی Settings ---
builder.Services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
builder.Services.Configure<MinioSettings>(configuration.GetSection("MinioSettings")); // استفاده از MinioSettings
builder.Services.Configure<KafkaSettings>(configuration.GetSection("KafkaSettings"));
builder.Services.Configure<CdnSettings>(configuration.GetSection("CdnSettings"));
builder.Services.Configure<RedisSettings>(configuration.GetSection("RedisSettings")); // افزودن تنظیمات Redis


// 2. --- پیکربندی MongoDB ---
// ثبت MongoClient به صورت Singleton
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    // تنظیمات پیشرفته‌تر کلاینت مانند Read/Write Concern را می‌توان اینجا اضافه کرد
    return new MongoClient(settings.ConnectionString);
});
// ثبت Repository ها (معمولا Scoped)
builder.Services.AddScoped<IProductRepository, MongoProductRepository>();

// 3. --- پیکربندی MinIO Client (با استفاده از AWS S3 SDK) ---
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var minioSettings = sp.GetRequiredService<IOptions<MinioSettings>>().Value;
    var config = new AmazonS3Config
    {
        ServiceURL = minioSettings.ServiceUrl, // آدرس MinIO
        ForcePathStyle = true, // مهم برای MinIO
        UseHttp = !minioSettings.UseHttps // استفاده از http یا https
        // SignatureVersion = "V4" // ممکن است لازم باشد
    };
    // استفاده از کلیدهای دسترسی MinIO
    var credentials = new BasicAWSCredentials(minioSettings.AccessKey, minioSettings.SecretKey);
    return new AmazonS3Client(credentials, config);
});


// 4. --- پیکربندی Kafka Producer (مانند قبل) ---
builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();


// 5. --- پیکربندی Redis ---
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisSettings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
    // اضافه کردن AbortOnConnectFail=false برای جلوگیری از خطا در استارت آپ اگر Redis در دسترس نباشد
    var configurationOptions = ConfigurationOptions.Parse(redisSettings.ConnectionString);
    configurationOptions.AbortOnConnectFail = redisSettings.AbortOnConnectFail;
    return ConnectionMultiplexer.Connect(configurationOptions);
});

// ثبت سرویس کشینگ (می‌توان مستقیم IDatabase را هم ثبت کرد)
builder.Services.AddSingleton<ICachingService, RedisCachingService>(); // Singleton برای کشینگ معمولا مناسب است



// 6. --- پیکربندی AutoMapper ---
builder.Services.AddAutoMapper(typeof(MappingProfile));

// 7. --- پیکربندی FluentValidation ---
builder.Services.AddFluentValidationAutoValidation(); // فعال‌سازی اعتبارسنجی خودکار
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>(); // ثبت تمام Validator ها در Assembly


// 8. --- پیکربندی HttpClient برای سرویس‌های دیگر با Polly ---
// تعریف یک Retry Policy پایه
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError() // مدیریت خطاهای 5xx, 408 و HttpRequestException
    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound) // Retry در صورت 404 (اختیاری)
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential back-off: 2s, 4s, 8s
        onRetry: (outcome, timespan, retryAttempt, context) => {
            context.GetLogger()?.LogWarning("Delaying for {delay}ms, then making retry {retry}.", timespan.TotalMilliseconds, retryAttempt);
        });

// تعریف یک Circuit Breaker Policy پایه
var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 5, // 5 خطای متوالی قبل از باز شدن مدار
        durationOfBreak: TimeSpan.FromSeconds(30) // مدار برای 30 ثانیه باز می‌ماند
    );

// ثبت HttpClient برای Discount Service با Policy ها
builder.Services.AddHttpClient<IDiscountServiceClient, DiscountServiceClient>(client =>
    {
        client.BaseAddress = new Uri(configuration["ServiceUrls:DiscountApi"] ?? throw new ArgumentNullException("ServiceUrls:DiscountApi not configured"));
        client.Timeout = TimeSpan.FromSeconds(10); // تایم اوت کلی درخواست
    })
    .AddPolicyHandler(retryPolicy) // اعمال Retry Policy
    .AddPolicyHandler(circuitBreakerPolicy); // اعمال Circuit Breaker Policy (بعد از Retry)

// ثبت HttpClient برای Inventory Service (اگر نیاز به فراخوانی آن باشد)
// builder.Services.AddHttpClient<IInventoryServiceClient, InventoryServiceClient>(...)
// .AddPolicyHandler(retryPolicy)
// .AddPolicyHandler(circuitBreakerPolicy);

// 9. --- ثبت سرویس‌ها ---
builder.Services.AddScoped<IProductService, ProductApi.Services.ProductService>();
builder.Services.AddScoped<IMediaService, MediaService>();


// 10. --- پیکربندی OpenTelemetry ---
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName: builder.Environment.ApplicationName))
    .WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation() // ردیابی درخواست‌های ورودی/خروجی ASP.NET Core
            .AddHttpClientInstrumentation() // ردیابی فراخوانی‌های HttpClient
            // .AddMongoDBInstrumentation() // نیاز به پکیج OpenTelemetry.Instrumentation.MongoDB (اگر موجود و سازگار باشد)
            // .AddConfluentKafkaInstrumentation() // نیاز به پکیج OpenTelemetry.Instrumentation.ConfluentKafka (اگر موجود و سازگار باشد)
            .AddConsoleExporter() // ارسال Trace ها به کنسول (برای تست)
        // .AddOtlpExporter(options => options.Endpoint = new Uri("http://jaeger:4317")) // ارسال به Jaeger یا OpenTelemetry Collector
    );


// 7. --- سایر سرویس‌های ASP.NET Core ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Product Service API (Refactored)", Version = "v1" });
    // فعال کردن خواندن کامنت‌های XML برای مستندسازی Swagger
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    // افزودن تنظیمات امنیتی JWT به Swagger (اگر از Authorize استفاده می‌کنید)
    // options.AddSecurityDefinition(...)
    // options.AddSecurityRequirement(...)
});

// (اختیاری) فعال کردن Health Checks
// builder.Services.AddHealthChecks()
//     .AddMongoDb(sp => sp.GetRequiredService<IOptions<MongoDbSettings>>().Value.ConnectionString, name: "mongodb")
//     .AddKafka(sp => {
//          var kafkaSettings = sp.GetRequiredService<IOptions<KafkaSettings>>().Value;
//          var config = new ProducerConfig { BootstrapServers = kafkaSettings.BootstrapServers };
//          return config;
//     }, name: "kafka")
//     .AddS3(sp => {
//         var s3Options = sp.GetRequiredService<IOptions<S3Settings>>().Value;
//         return new Amazon.S3.AmazonS3Config { RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(s3Options.Region) };
//         // تنظیمات Credential برای Health Check...
//     }, name: "s3");


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Service API v1"));
    app.UseDeveloperExceptionPage(); // نمایش خطاهای دقیق در محیط توسعه
}
else
{
    // UseExceptionHandler, HSTS در محیط Production
    app.UseHsts();
}

app.UseHttpsRedirection();

// app.UseRouting(); // UseRouting قبل از UseAuthentication و UseAuthorization

// app.UseAuthentication(); // فعال کردن احراز هویت (اگر دارید)
app.UseAuthorization();

// app.MapHealthChecks("/health"); // فعال کردن Endpoint برای Health Checks

app.MapControllers();

app.Run();


