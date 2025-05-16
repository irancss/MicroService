using CartApi.Repo;
using CartApi.Services;
using StackExchange.Redis;
using Polly;
using static CartApi.Services.CartService;
using Confluent.Kafka;
using CartApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. پیکربندی Redis

var redisSettings = builder.Configuration.GetSection("Redis").Get<RedisSettings>();
if (redisSettings == null || string.IsNullOrEmpty(redisSettings.ConnectionString))
{
    throw new InvalidOperationException("Redis configuration is missing.");
}

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisSettings.ConnectionString)
);

// 2. ثبت Repository و Service
builder.Services.AddScoped<ICartRepository, RedisCartRepository>(); // Scoped یا Transient مناسب است
builder.Services.AddScoped<ICartService, CartService>();


#region Polly

builder.Services.AddHttpClient<IProductServiceClient, ProductServiceClient>()
    .AddTransientHttpErrorPolicy(policy =>
        policy.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)))
    .AddTransientHttpErrorPolicy(policy =>
        policy.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

#endregion

#region Kafka

var kafkaConfig = new ProducerConfig();
builder.Configuration.Bind("Kafka:ProducerSettings", kafkaConfig);
builder.Services.AddSingleton(kafkaConfig);
builder.Services.AddSingleton<KafkaProducerService>();

var consumerConfig = new ConsumerConfig();
builder.Configuration.Bind("Kafka:ConsumerSettings", consumerConfig);
builder.Services.AddSingleton(consumerConfig);
builder.Services.AddHostedService<KafkaConsumerService>();

#endregion
// 3. (اختیاری) ثبت کلاینت‌های HTTP برای سرویس‌های دیگر
// builder.Services.AddHttpClient<IProductServiceClient, ProductServiceClient>(client =>
// {
//     client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductApi"]);
// });
// builder.Services.AddHttpClient<IDiscountServiceClient, DiscountServiceClient>(client =>
// {
//    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:DiscountApi"]);
// });


// 4. (اختیاری) پیکربندی Kafka Producer/Consumer
/*
// Producer Config
var producerConfig = new ProducerConfig();
builder.Configuration.Bind("Kafka:ProducerSettings", producerConfig);
builder.Services.AddSingleton<IProducer<Null, string>>(new ProducerBuilder<Null, string>(producerConfig).Build());
builder.Services.AddSingleton<KafkaProducerService>(); // سرویس خودتان برای ارسال پیام

// Consumer Config (برای گوش دادن به رویداد اتمام سفارش)
var consumerConfig = new ConsumerConfig();
builder.Configuration.Bind("Kafka:ConsumerSettings", consumerConfig);
// تنظیم GroupId ضروری است
consumerConfig.GroupId = "cart-service-group";
consumerConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
builder.Services.AddSingleton(consumerConfig); // کانفیگ را ثبت کنید
builder.Services.AddHostedService<KafkaOrderCompletedConsumer>(); // سرویس پس‌زمینه برای گوش دادن
*/

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
    .AddRedis(builder.Configuration.GetValue<string>("Redis:ConnectionString"))
    .AddKafka(new ProducerConfig { BootstrapServers = "localhost:9092" });

var app = builder.Build();

app.MapHealthChecks("/health");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
