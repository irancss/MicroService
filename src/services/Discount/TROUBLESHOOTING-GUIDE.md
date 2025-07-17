# راهنمای عیب‌یابی (Troubleshooting) سرویس تخفیف

## 📋 فهرست

1. [خطاهای رایج و راه‌حل](#common-errors)
2. [مشکلات پایگاه داده](#database-issues)
3. [مشکلات Cache و Redis](#cache-issues)
4. [مشکلات Message Queue](#messagequeue-issues)
5. [مشکلات Performance](#performance-issues)
6. [ابزارهای Debugging](#debugging-tools)
7. [لاگ‌ها و Monitoring](#logs-monitoring)

## 🚨 خطاهای رایج و راه‌حل

### 1. خطای Authentication

#### مشکل:
```json
{
  "error": "Unauthorized",
  "message": "JWT token is invalid or expired",
  "statusCode": 401
}
```

#### راه‌حل:
```bash
# بررسی تنظیمات JWT
echo $JWT_SECRET
echo $JWT_ISSUER
echo $JWT_AUDIENCE

# تست JWT token
curl -H "Authorization: Bearer YOUR_TOKEN" \
  https://jwt.io/

# بررسی expiration
dotnet user-jwts print YOUR_TOKEN
```

#### عیب‌یابی:
```csharp
// اضافه کردن لاگ در JWT middleware
app.Use(async (context, next) =>
{
    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
    if (!string.IsNullOrEmpty(token))
    {
        logger.LogInformation("Received JWT Token: {Token}", token);
        
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            logger.LogInformation("Token expires at: {Expiry}", jsonToken.ValidTo);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Invalid JWT token format");
        }
    }
    
    await next();
});
```

### 2. خطای Validation

#### مشکل:
```json
{
  "errors": {
    "Value": ["The field Value must be between 0 and 100."],
    "EndDate": ["End date must be after start date."]
  },
  "statusCode": 400
}
```

#### راه‌حل:
```csharp
// بررسی validator rules
public class CreateDiscountCommandValidator : AbstractValidator<CreateDiscountCommand>
{
    public CreateDiscountCommandValidator()
    {
        RuleFor(x => x.Value)
            .GreaterThan(0).WithMessage("مقدار تخفیف باید بیشتر از صفر باشد")
            .LessThanOrEqualTo(100).When(x => x.Type == DiscountType.Percentage)
            .WithMessage("تخفیف درصدی نمی‌تواند بیش از 100% باشد");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("تاریخ پایان باید بعد از تاریخ شروع باشد");
    }
}
```

### 3. خطای Database Connection

#### مشکل:
```
Npgsql.NpgsqlException: Connection refused at localhost:5432
```

#### راه‌حل:
```bash
# بررسی وضعیت PostgreSQL
docker ps | grep postgres
docker logs discount-postgres

# تست اتصال manual
psql -h localhost -p 5432 -U discount_user -d discount_db

# بررسی connection string
echo $POSTGRES_CONNECTION_STRING

# restart database container
docker restart discount-postgres
```

#### Connection Pool Issues:
```csharp
// تنظیم Connection Pool در appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=discount_db;Username=discount_user;Password=password;Pooling=true;MinPoolSize=0;MaxPoolSize=100;ConnectionIdleLifetime=300"
  }
}
```

## 🗄️ مشکلات پایگاه داده

### 1. Migration Failures

#### مشکل:
```
System.InvalidOperationException: The migration '20240315_InitialCreate' has already been applied to the database.
```

#### راه‌حل:
```bash
# بررسی وضعیت migrations
dotnet ef migrations list

# حذف migration آخر
dotnet ef migrations remove

# اعمال مجدد migrations
dotnet ef database update

# در صورت نیاز، reset کامل database
dotnet ef database drop
dotnet ef database update
```

### 2. Index Performance Issues

#### مشکل:
Query های کند روی جدول Discounts

#### تشخیص:
```sql
-- بررسی query plan
EXPLAIN ANALYZE 
SELECT * FROM "Discounts" 
WHERE "IsActive" = true 
  AND "StartDate" <= NOW() 
  AND "EndDate" >= NOW();

-- بررسی missing indexes
SELECT schemaname, tablename, attname, n_distinct, correlation 
FROM pg_stats 
WHERE tablename = 'Discounts';
```

#### راه‌حل:
```sql
-- اضافه کردن index composite
CREATE INDEX CONCURRENTLY IX_Discounts_Active_DateRange 
ON "Discounts" ("IsActive", "StartDate", "EndDate") 
WHERE "IsActive" = true;

-- Index برای CouponCode
CREATE UNIQUE INDEX CONCURRENTLY IX_Discounts_CouponCode 
ON "Discounts" ("CouponCode") 
WHERE "CouponCode" IS NOT NULL;
```

### 3. Deadlock Issues

#### مشکل:
```
Npgsql.PostgresException: deadlock detected
```

#### تشخیص:
```sql
-- مانیتور locks
SELECT 
    blocked_locks.pid AS blocked_pid,
    blocked_activity.usename AS blocked_user,
    blocking_locks.pid AS blocking_pid,
    blocking_activity.usename AS blocking_user,
    blocked_activity.query AS blocked_statement,
    blocking_activity.query AS current_statement_in_blocking_process
FROM pg_catalog.pg_locks blocked_locks
JOIN pg_catalog.pg_stat_activity blocked_activity 
    ON blocked_activity.pid = blocked_locks.pid
JOIN pg_catalog.pg_locks blocking_locks 
    ON blocking_locks.locktype = blocked_locks.locktype
    AND blocking_locks.database IS NOT DISTINCT FROM blocked_locks.database
    AND blocking_locks.relation IS NOT DISTINCT FROM blocked_locks.relation
    AND blocking_locks.page IS NOT DISTINCT FROM blocked_locks.page
    AND blocking_locks.tuple IS NOT DISTINCT FROM blocked_locks.tuple
    AND blocking_locks.virtualxid IS NOT DISTINCT FROM blocked_locks.virtualxid
    AND blocking_locks.transactionid IS NOT DISTINCT FROM blocked_locks.transactionid
    AND blocking_locks.classid IS NOT DISTINCT FROM blocked_locks.classid
    AND blocking_locks.objid IS NOT DISTINCT FROM blocked_locks.objid
    AND blocking_locks.objsubid IS NOT DISTINCT FROM blocked_locks.objsubid
    AND blocking_locks.pid != blocked_locks.pid
JOIN pg_catalog.pg_stat_activity blocking_activity 
    ON blocking_activity.pid = blocking_locks.pid
WHERE NOT blocked_locks.granted;
```

#### راه‌حل:
```csharp
// استفاده از transaction scope مناسب
public async Task<Discount> UpdateDiscountUsageAsync(Guid discountId, Guid userId)
{
    using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
    
    try
    {
        // قفل کردن رکورد خاص
        var discount = await _context.Discounts
            .Where(d => d.Id == discountId)
            .FirstOrDefaultAsync();
            
        if (discount == null)
            throw new NotFoundException("Discount not found");
            
        // به‌روزرسانی atomic
        discount.CurrentTotalUsage++;
        
        // اضافه کردن usage history
        var usage = new DiscountUsageHistory
        {
            DiscountId = discountId,
            UserId = userId,
            UsedAt = DateTime.UtcNow
        };
        
        _context.DiscountUsageHistories.Add(usage);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        
        return discount;
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```

## 🔄 مشکلات Cache و Redis

### 1. Redis Connection Issues

#### مشکل:
```
StackExchange.Redis.RedisConnectionException: No connection is available to service this operation
```

#### تشخیص:
```bash
# بررسی وضعیت Redis
docker logs discount-redis
redis-cli ping

# بررسی connection string
echo $REDIS_CONNECTION_STRING

# تست اتصال
redis-cli -h localhost -p 6379 -a your_password ping
```

#### راه‌حل:
```csharp
// تنظیم Connection Options
services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    var configuration = provider.GetService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("Redis");
    
    var options = ConfigurationOptions.Parse(connectionString);
    options.AbortOnConnectFail = false;
    options.ConnectRetry = 3;
    options.ConnectTimeout = 10000;
    options.SyncTimeout = 10000;
    options.KeepAlive = 180;
    
    return ConnectionMultiplexer.Connect(options);
});
```

### 2. Cache Invalidation Issues

#### مشکل:
داده‌های قدیمی در cache باقی می‌ماند

#### تشخیص:
```bash
# بررسی keys موجود در Redis
redis-cli --scan --pattern "discount:*"

# چک کردن TTL
redis-cli TTL "discount:active"

# مشاهده memory usage
redis-cli INFO memory
```

#### راه‌حل:
```csharp
// Pattern برای invalidation
public class RedisCacheService : ICacheService
{
    public async Task InvalidatePatternAsync(string pattern)
    {
        var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
        var keys = server.Keys(pattern: pattern);
        
        var database = _connectionMultiplexer.GetDatabase();
        var tasks = keys.Select(key => database.KeyDeleteAsync(key));
        
        await Task.WhenAll(tasks);
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var database = _connectionMultiplexer.GetDatabase();
        var serializedValue = JsonSerializer.Serialize(value);
        
        await database.StringSetAsync(key, serializedValue, expiry);
        
        // ثبت لاگ برای debugging
        _logger.LogDebug("Cache set: {Key} with expiry {Expiry}", key, expiry);
    }
}
```

### 3. Memory Issues در Redis

#### مشکل:
```
Redis OOM (Out Of Memory)
```

#### تشخیص:
```bash
# بررسی memory usage
redis-cli INFO memory

# بررسی largest keys
redis-cli --bigkeys

# مانیتور real-time
redis-cli MONITOR
```

#### راه‌حل:
```bash
# تنظیم memory policy
redis-cli CONFIG SET maxmemory-policy allkeys-lru

# پاک کردن expired keys
redis-cli --eval "return redis.call('del', unpack(redis.call('keys', 'expired:*')))" 0

# تنظیم حداکثر memory
redis-cli CONFIG SET maxmemory 1gb
```

## 📨 مشکلات Message Queue

### 1. RabbitMQ Connection Issues

#### مشکل:
```
RabbitMQ.Client.Exceptions.BrokerUnreachableException: None of the specified endpoints were reachable
```

#### تشخیص:
```bash
# بررسی وضعیت RabbitMQ
docker logs discount-rabbitmq
curl http://localhost:15672/api/overview

# بررسی users و permissions
curl -u admin:password http://localhost:15672/api/users
curl -u admin:password http://localhost:15672/api/permissions
```

#### راه‌حل:
```csharp
// Retry policy برای MassTransit
services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("discount_user");
            h.Password("password");
        });
        
        // Retry configuration
        cfg.UseRetry(r => r.Intervals(
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(10)
        ));
        
        // Circuit breaker
        cfg.UseCircuitBreaker(cb =>
        {
            cb.TrackingPeriod = TimeSpan.FromMinutes(1);
            cb.TripThreshold = 15;
            cb.ActiveThreshold = 10;
            cb.ResetInterval = TimeSpan.FromMinutes(5);
        });
    });
});
```

### 2. Message Processing Failures

#### مشکل:
Messages در error queue جمع شده‌اند

#### تشخیص:
```bash
# بررسی queue ها
curl -u admin:password http://localhost:15672/api/queues

# بررسی message count
curl -u admin:password http://localhost:15672/api/queues/%2F/discount_events

# مشاهده error queue
curl -u admin:password http://localhost:15672/api/queues/%2F/discount_events_error
```

#### راه‌حل:
```csharp
// Error handling در consumer
public class DiscountUsedEventConsumer : IConsumer<DiscountUsedEvent>
{
    private readonly ILogger<DiscountUsedEventConsumer> _logger;
    
    public async Task Consume(ConsumeContext<DiscountUsedEvent> context)
    {
        try
        {
            var @event = context.Message;
            _logger.LogInformation("Processing DiscountUsedEvent for discount {DiscountId}", @event.DiscountId);
            
            // Process event
            await ProcessDiscountUsedEvent(@event);
            
            _logger.LogInformation("Successfully processed DiscountUsedEvent");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing DiscountUsedEvent: {Message}", ex.Message);
            
            // اگر critical error است، reject کن
            if (ex is ArgumentException)
            {
                throw; // برو به error queue
            }
            
            // اگر temporary error است، retry کن
            throw new RetryException("Temporary failure, will retry");
        }
    }
}
```

## ⚡ مشکلات Performance

### 1. Slow API Responses

#### تشخیص:
```csharp
// اضافه کردن performance monitoring
app.Use(async (context, next) =>
{
    var stopwatch = Stopwatch.StartNew();
    
    await next();
    
    stopwatch.Stop();
    var elapsedMs = stopwatch.ElapsedMilliseconds;
    
    if (elapsedMs > 1000) // بیش از 1 ثانیه
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogWarning("Slow request: {Method} {Path} took {ElapsedMs}ms", 
            context.Request.Method, 
            context.Request.Path, 
            elapsedMs);
    }
    
    context.Response.Headers.Add("X-Response-Time", $"{elapsedMs}ms");
});
```

#### راه‌حل:
```csharp
// بهینه‌سازی queries
public async Task<IEnumerable<Discount>> GetActiveDiscountsAsync()
{
    return await _context.Discounts
        .Where(d => d.IsActive && 
                   d.StartDate <= DateTime.UtcNow && 
                   d.EndDate >= DateTime.UtcNow)
        .AsNoTracking() // برای read-only queries
        .ToListAsync();
}

// استفاده از pagination
public async Task<PagedResult<Discount>> GetDiscountsPagedAsync(int pageNumber, int pageSize)
{
    var query = _context.Discounts.AsQueryable();
    
    var totalCount = await query.CountAsync();
    
    var discounts = await query
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .AsNoTracking()
        .ToListAsync();
    
    return new PagedResult<Discount>
    {
        Items = discounts,
        TotalCount = totalCount,
        PageNumber = pageNumber,
        PageSize = pageSize
    };
}
```

### 2. Memory Leaks

#### تشخیص:
```bash
# مانیتور memory usage
docker stats discount-api

# بررسی GC در .NET
curl http://localhost:8080/metrics | grep dotnet_gc
```

#### راه‌حل:
```csharp
// Proper disposal of resources
public class DiscountService : IDisposable
{
    private readonly HttpClient _httpClient;
    private bool _disposed = false;
    
    public DiscountService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
            }
            _disposed = true;
        }
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

// استفاده از ObjectPool برای objects پرتکرار
services.AddSingleton<ObjectPool<StringBuilder>>(provider =>
{
    var provider = new DefaultObjectPoolProvider();
    return provider.Create<StringBuilder>();
});
```

## 🔍 ابزارهای Debugging

### 1. Application Insights

```csharp
// تنظیم Application Insights
services.AddApplicationInsightsTelemetry();

// Custom telemetry
public class DiscountController : ControllerBase
{
    private readonly TelemetryClient _telemetryClient;
    
    [HttpPost("calculate")]
    public async Task<IActionResult> CalculateDiscount([FromBody] CalculateDiscountRequest request)
    {
        using var operation = _telemetryClient.StartOperation<DependencyTelemetry>("CalculateDiscount");
        
        try
        {
            // Business logic
            var result = await _discountService.CalculateDiscountAsync(request);
            
            _telemetryClient.TrackEvent("DiscountCalculated", new Dictionary<string, string>
            {
                ["UserId"] = request.UserId.ToString(),
                ["DiscountAmount"] = result.DiscountAmount.ToString(),
                ["CouponCode"] = request.CouponCode
            });
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _telemetryClient.TrackException(ex);
            throw;
        }
    }
}
```

### 2. Structured Logging با Serilog

```csharp
// تنظیم Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/discount-service-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://localhost:5341")
    .WriteTo.ApplicationInsights(configuration["ApplicationInsights:InstrumentationKey"], TelemetryConverter.Traces)
    .CreateLogger();

// استفاده در کد
_logger.LogInformation("Calculating discount for user {UserId} with {ItemCount} items", 
    request.UserId, 
    request.Items.Count);

_logger.LogWarning("Discount {DiscountId} usage limit exceeded. Current usage: {CurrentUsage}, Max: {MaxUsage}",
    discount.Id,
    discount.CurrentTotalUsage,
    discount.MaxTotalUsage);
```

### 3. Health Checks تفصیلی

```csharp
// Custom health checks
public class DiscountServiceHealthCheck : IHealthCheck
{
    private readonly IDiscountRepository _repository;
    private readonly ICacheService _cacheService;
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // تست database
            await _repository.GetActiveDiscountsAsync();
            
            // تست cache
            await _cacheService.GetAsync<string>("health-check");
            
            return HealthCheckResult.Healthy("All systems operational");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Service is unhealthy", ex);
        }
    }
}

// ثبت health checks
services.AddHealthChecks()
    .AddCheck<DiscountServiceHealthCheck>("discount-service")
    .AddDbContextCheck<DiscountDbContext>()
    .AddRedis(configuration.GetConnectionString("Redis"))
    .AddRabbitMQ(configuration.GetConnectionString("RabbitMQ"));
```

## 📊 لاگ‌ها و Monitoring

### نمونه لاگ‌های مهم:

```json
{
  "timestamp": "2024-03-15T10:30:00Z",
  "level": "Information",
  "messageTemplate": "Discount calculated successfully",
  "properties": {
    "UserId": "123e4567-e89b-12d3-a456-426614174000",
    "DiscountId": "123e4567-e89b-12d3-a456-426614174001",
    "OriginalAmount": 1500000,
    "DiscountAmount": 300000,
    "FinalAmount": 1200000,
    "CouponCode": "SAVE20",
    "ProcessingTimeMs": 45
  }
}
```

### Queries مفید در Seq/ElasticSearch:

```sql
-- تمام خطاهای 24 ساعت گذشته
@Level = 'Error' AND @Timestamp > Now() - 1d

-- Slow queries
@MessageTemplate like '%slow%' OR ProcessingTimeMs > 1000

-- Failed discount calculations
@MessageTemplate like '%discount calculation failed%'

-- Cache misses
@MessageTemplate like '%cache miss%'
```

---

**نکته:** برای سریع‌تر یافتن مشکلات، همواره از structured logging استفاده کنید و metrics مهم را monitor کنید.
