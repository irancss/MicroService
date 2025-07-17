# راهنمای تست سرویس تخفیف

## 📋 فهرست

1. [تست‌های یونیت](#unit-tests)
2. [تست‌های یکپارچگی](#integration-tests)
3. [تست‌های API](#api-tests)
4. [تست‌های عملکرد](#performance-tests)
5. [سناریوهای تست](#test-scenarios)

## 🧪 تست‌های یونیت

### محیط تست

#### نصب ابزارهای مورد نیاز:
```bash
# اضافه کردن پکیج‌های تست
dotnet add DiscountService.Tests package xUnit
dotnet add DiscountService.Tests package xunit.runner.visualstudio
dotnet add DiscountService.Tests package Microsoft.NET.Test.Sdk
dotnet add DiscountService.Tests package Moq
dotnet add DiscountService.Tests package FluentAssertions
dotnet add DiscountService.Tests package Microsoft.EntityFrameworkCore.InMemory
```

### تست Domain Layer

#### تست محاسبه تخفیف درصدی:
```csharp
public class DiscountCalculationServiceTests
{
    [Fact]
    public void CalculatePercentageDiscount_ShouldReturnCorrectAmount()
    {
        // Arrange
        var service = new DiscountCalculationService();
        var discount = new Discount
        {
            Type = DiscountType.Percentage,
            Value = 20,
            MaxDiscountAmount = 1000000
        };
        var cart = CreateSampleCart(5000000); // 5 میلیون تومان

        // Act
        var result = service.CalculateDiscount(discount, cart);

        // Assert
        result.DiscountAmount.Should().Be(1000000); // 20% اما محدود به حداکثر
        result.IsApplicable.Should().BeTrue();
    }

    [Theory]
    [InlineData(1000000, 200000)] // 1 میلیون -> 200 هزار تخفیف
    [InlineData(3000000, 600000)] // 3 میلیون -> 600 هزار تخفیف
    [InlineData(10000000, 1000000)] // 10 میلیون -> 1 میلیون (محدود شده)
    public void CalculatePercentageDiscount_WithDifferentAmounts_ShouldReturnExpectedDiscount(
        decimal cartTotal, decimal expectedDiscount)
    {
        // Test implementation
    }
}
```

#### تست قوانین کسب و کار:
```csharp
public class DiscountBusinessRulesTests
{
    [Fact]
    public void ValidateDiscount_ExpiredDiscount_ShouldReturnFalse()
    {
        // Arrange
        var discount = new Discount
        {
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(-1),
            IsActive = true
        };

        // Act
        var isValid = discount.IsCurrentlyValid();

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateMinimumCartAmount_InsufficientAmount_ShouldReturnFalse()
    {
        // Test implementation
    }
}
```

### تست Application Layer

#### تست Command Handlers:
```csharp
public class CreateDiscountCommandHandlerTests
{
    private readonly Mock<IDiscountRepository> _repositoryMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly CreateDiscountCommandHandler _handler;

    public CreateDiscountCommandHandlerTests()
    {
        _repositoryMock = new Mock<IDiscountRepository>();
        _cacheMock = new Mock<ICacheService>();
        _handler = new CreateDiscountCommandHandler(_repositoryMock.Object, _cacheMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateDiscount()
    {
        // Arrange
        var command = new CreateDiscountCommand
        {
            Name = "تست تخفیف",
            Type = DiscountType.Percentage,
            Value = 15,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Discount>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("تست تخفیف");
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Discount>()), Times.Once);
    }
}
```

## 🔗 تست‌های یکپارچگی

### تست Database Integration:

```csharp
public class DiscountRepositoryIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public DiscountRepositoryIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetActiveDiscountsAsync_ShouldReturnOnlyActiveDiscounts()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new DiscountRepository(context);

        // Seed test data
        await SeedTestData(context);

        // Act
        var activeDiscounts = await repository.GetActiveDiscountsAsync();

        // Assert
        activeDiscounts.Should().HaveCountGreaterThan(0);
        activeDiscounts.Should().OnlyContain(d => d.IsActive);
    }

    private async Task SeedTestData(DiscountDbContext context)
    {
        var discounts = new[]
        {
            new Discount
            {
                Id = Guid.NewGuid(),
                Name = "فعال",
                IsActive = true,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1)
            },
            new Discount
            {
                Id = Guid.NewGuid(),
                Name = "غیرفعال",
                IsActive = false,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1)
            }
        };

        context.Discounts.AddRange(discounts);
        await context.SaveChangesAsync();
    }
}
```

### Database Fixture:
```csharp
public class DatabaseFixture : IDisposable
{
    private readonly string _connectionString;

    public DatabaseFixture()
    {
        _connectionString = "Host=localhost;Database=DiscountTestDb;Username=postgres;Password=postgres";
        
        // Initialize test database
        using var context = CreateContext();
        context.Database.EnsureCreated();
    }

    public DiscountDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DiscountDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        return new DiscountDbContext(options);
    }

    public void Dispose()
    {
        using var context = CreateContext();
        context.Database.EnsureDeleted();
    }
}
```

## 🌐 تست‌های API

### تست Controller:
```csharp
public class DiscountControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public DiscountControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CalculateDiscount_ValidRequest_ShouldReturnDiscount()
    {
        // Arrange
        var request = new CalculateDiscountRequest
        {
            UserId = Guid.NewGuid(),
            Items = new[]
            {
                new CartItemRequest
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "تست محصول",
                    CategoryId = Guid.NewGuid(),
                    CategoryName = "تست دسته",
                    UnitPrice = 1000000,
                    Quantity = 1
                }
            },
            ShippingCost = 50000,
            CouponCode = "TEST20"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/discounts/calculate", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<DiscountCalculationResponse>(responseContent);
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetDiscounts_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/admin/discounts");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
```

### تست با Authentication:
```csharp
public class AdminDiscountControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly string _adminToken;

    public AdminDiscountControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _adminToken = GenerateAdminToken();
    }

    [Fact]
    public async Task CreateDiscount_ValidRequest_ShouldReturnCreated()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _adminToken);

        var request = new CreateDiscountRequest
        {
            Name = "تخفیف تست",
            Description = "تخفیف برای تست",
            Type = DiscountType.Percentage,
            Value = 15,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/admin/discounts", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    private string GenerateAdminToken()
    {
        // JWT token generation logic for testing
        return "test_admin_token";
    }
}
```

## ⚡ تست‌های عملکرد

### تست Cache Performance:
```csharp
public class CachePerformanceTests
{
    [Fact]
    public async Task GetActiveDiscounts_WithCache_ShouldBeFaster()
    {
        // Arrange
        var repository = new Mock<IDiscountRepository>();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var cacheService = new RedisCacheService(cache);

        // First call - no cache
        var stopwatch1 = Stopwatch.StartNew();
        await GetActiveDiscountsWithoutCache(repository.Object);
        stopwatch1.Stop();

        // Second call - with cache
        var stopwatch2 = Stopwatch.StartNew();
        await GetActiveDiscountsWithCache(cacheService);
        stopwatch2.Stop();

        // Assert
        stopwatch2.ElapsedMilliseconds.Should().BeLessThan(stopwatch1.ElapsedMilliseconds);
    }
}
```

### Load Testing با NBomber:
```csharp
public class LoadTests
{
    [Fact]
    public void DiscountCalculation_LoadTest()
    {
        var scenario = Scenario.Create("discount_calculation", async context =>
        {
            var httpClient = new HttpClient();
            var request = CreateTestRequest();
            
            var response = await httpClient.PostAsync(
                "http://localhost:8080/api/discounts/calculate", 
                CreateJsonContent(request));

            return Response.Ok();
        })
        .WithLoadSimulations(
            Simulation.InjectPerSec(rate: 50, during: TimeSpan.FromMinutes(5))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
```

## 📝 سناریوهای تست

### سناریو 1: تخفیف درصدی ساده
```bash
# Test Case: محاسبه تخفیف 20%
curl -X POST http://localhost:8080/api/discounts/calculate \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "123e4567-e89b-12d3-a456-426614174000",
    "items": [
      {
        "productId": "123e4567-e89b-12d3-a456-426614174001",
        "productName": "محصول تست",
        "categoryId": "123e4567-e89b-12d3-a456-426614174002",
        "categoryName": "دسته تست",
        "unitPrice": 1000000,
        "quantity": 1
      }
    ],
    "shippingCost": 50000,
    "couponCode": "TEST20"
  }'

# Expected Result:
# - originalTotal: 1050000
# - discountAmount: 200000 (20% از 1000000)
# - finalTotal: 850000
```

### سناریو 2: تخفیف BOGO
```bash
# Test Case: خرید 2 عدد، 1 عدد رایگان
curl -X POST http://localhost:8080/api/discounts/calculate \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "123e4567-e89b-12d3-a456-426614174000",
    "items": [
      {
        "productId": "123e4567-e89b-12d3-a456-426614174001",
        "productName": "کفش ورزشی",
        "categoryId": "123e4567-e89b-12d3-a456-426614174002",
        "categoryName": "کفش",
        "unitPrice": 500000,
        "quantity": 3
      }
    ],
    "shippingCost": 50000,
    "couponCode": "BOGO_SHOES"
  }'

# Expected Result:
# - originalTotal: 1550000
# - discountAmount: 500000 (1 عدد رایگان)
# - finalTotal: 1050000
```

### سناریو 3: ارسال رایگان
```bash
# Test Case: ارسال رایگان برای خرید بالای 1 میلیون
curl -X POST http://localhost:8080/api/discounts/calculate \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "123e4567-e89b-12d3-a456-426614174000",
    "items": [
      {
        "productId": "123e4567-e89b-12d3-a456-426614174001",
        "productName": "لپ تاپ",
        "categoryId": "123e4567-e89b-12d3-a456-426614174002",
        "categoryName": "الکترونیک",
        "unitPrice": 1200000,
        "quantity": 1
      }
    ],
    "shippingCost": 80000
  }'

# Expected Result:
# - originalTotal: 1280000
# - shippingDiscount: 80000
# - finalTotal: 1200000
```

## 🎯 Test Coverage Goals

### حداقل Coverage مورد نیاز:
- **Domain Layer**: 95%
- **Application Layer**: 90%
- **Infrastructure Layer**: 80%
- **API Layer**: 85%

### اجرای Coverage Report:
```bash
# نصب ابزار coverage
dotnet tool install --global dotnet-reportgenerator-globaltool

# اجرای تست با coverage
dotnet test --collect:"XPlat Code Coverage"

# تولید گزارش HTML
reportgenerator \
  -reports:"**/coverage.cobertura.xml" \
  -targetdir:"CoverageReport" \
  -reporttypes:Html

# مشاهده گزارش
start CoverageReport/index.html
```

## 🚀 CI/CD Pipeline Testing

### GitHub Actions Configuration:
```yaml
name: Test Pipeline

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    
    services:
      postgres:
        image: postgres:15
        env:
          POSTGRES_PASSWORD: postgres
          POSTGRES_DB: discount_test
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5

    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 8.0.x
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Test
      run: dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"
    
    - name: Upload coverage to Codecov
      uses: codecov/codecov-action@v3
```

## 📊 Test Data Management

### Test Database Seeding:
```csharp
public static class TestDataSeeder
{
    public static async Task SeedDiscounts(DiscountDbContext context)
    {
        var discounts = new[]
        {
            new Discount
            {
                Id = Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
                Name = "تخفیف تست 20%",
                Type = DiscountType.Percentage,
                Value = 20,
                CouponCode = "TEST20",
                IsActive = true,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(30)
            },
            // سایر تخفیف‌های تست
        };

        context.Discounts.AddRange(discounts);
        await context.SaveChangesAsync();
    }
}
```

---

**نکته مهم:** همواره تست‌ها را قبل از merge کردن کد اجرا کنید و مطمئن شوید که coverage مطلوب حاصل شده است.
