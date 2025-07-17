# IdentityServer8 Microservice

این پروژه یک پیاده‌سازی کامل از IdentityServer8 برای استفاده در معماری میکروسرویس است. این سرویس امکانات احراز هویت و اجازه‌دهی مبتنی بر OpenID Connect و OAuth 2.0 را فراهم می‌کند.

## ویژگی‌های کلیدی

- **OpenID Connect & OAuth 2.0**: پشتیبانی کامل از استانداردهای احراز هویت مدرن
- **چندین نوع Client**: پشتیبانی از وب اپلیکیشن‌ها، موبایل اپ‌ها، SPA ها و ارتباط machine-to-machine
- **Test Users**: کاربران تست آماده برای توسعه
- **مبتنی بر .NET 8**: عملکرد بالا و مقیاس‌پذیری
- **Logging با Serilog**: لاگ‌گیری پیشرفته برای monitoring و debugging
- **Docker Ready**: آماده برای containerization

## کانفیگوریشن

### API Scopes
- `microservice.api`: دسترسی عمومی به API های میکروسرویس
- `user.api`: مدیریت کاربران
- `order.api`: مدیریت سفارشات
- `payment.api`: پردازش پرداخت
- `notification.api`: سرویس اعلانات
- `full_access`: دسترسی کامل (فقط برای Admin)

### Clients

#### 1. Microservice Client (Machine-to-Machine)
```
ClientId: microservice.client
Secret: microservice_secret_2024
Grant Type: Client Credentials
```

#### 2. Web Application Client
```
ClientId: web.client
Secret: web_secret_2024
Grant Type: Authorization Code
```

#### 3. Mobile/SPA Client
```
ClientId: mobile.client
Grant Type: Authorization Code + PKCE
Require Secret: false
```

#### 4. Admin Client
```
ClientId: admin.client
Secret: admin_secret_2024
Grant Type: Authorization Code
Scopes: full_access
```

### Test Users

| نام کاربری | رمز عبور | نقش |
|-----------|---------|------|
| admin | admin123 | Administrator |
| manager | manager123 | Manager |
| user | user123 | User |

## نحوه اجرا

### پیش‌نیازها
- .NET 8 SDK
- Visual Studio 2022 یا VS Code

### مراحل اجرا

1. **کلون کردن و نصب packages**:
```bash
cd src/services/IdentityServer
dotnet restore
```

2. **اجرای پروژه**:
```bash
dotnet run
```

3. **دسترسی به سرویس**:
- Home Page: https://localhost:5000
- Discovery Document: https://localhost:5000/.well-known/openid_configuration
- Health Check: https://localhost:5000/health

## استفاده در میکروسرویس‌ها

### نحوه احراز هویت Token

```csharp
// در سایر میکروسرویس‌ها
services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:5000";
        options.RequireHttpsMetadata = false; // فقط برای development
        
        options.Audience = "microservice";
    });
```

### درخواست Token (Client Credentials)

```csharp
var client = new HttpClient();
var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5000");

var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
    Address = disco.TokenEndpoint,
    ClientId = "microservice.client",
    ClientSecret = "microservice_secret_2024",
    Scope = "user.api order.api"
});
```

### استفاده از Token در API Calls

```csharp
var client = new HttpClient();
client.SetBearerToken(tokenResponse.AccessToken);

var response = await client.GetAsync("https://localhost:6001/api/users");
```

## کانفیگوریشن برای Production

### 1. تغییر Signing Certificate
```csharp
// در Program.cs
identityServerBuilder.AddSigningCredential(GetCertificate());

private static X509Certificate2 GetCertificate()
{
    // بارگیری certificate از file یا certificate store
}
```

### 2. استفاده از Database به جای In-Memory
```csharp
// نصب package EntityFramework
// Install-Package HigginsSoft.IdentityServer8.EntityFramework

// کانفیگوریشن
identityServerBuilder
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(connectionString);
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(connectionString);
    });
```

### 3. تنظیم User Store واقعی
```csharp
// برای ASP.NET Core Identity
identityServerBuilder.AddAspNetIdentity<ApplicationUser>();
```

## Endpoints

- **Discovery**: `/.well-known/openid_configuration`
- **Authorization**: `/connect/authorize`
- **Token**: `/connect/token`
- **UserInfo**: `/connect/userinfo`
- **End Session**: `/connect/endsession`
- **Introspection**: `/connect/introspect`

## Security Notes

⚠️ **مهم**: این کانفیگوریشن برای development است. برای production:

1. از certificate های واقعی استفاده کنید
2. secrets ها را در environment variables قرار دهید
3. از database برای ذخیره کانفیگوریشن استفاده کنید
4. HTTPS را الزامی کنید
5. CORS policy ها را محدود کنید

## مرتبط با میکروسرویس‌ها

این IdentityServer می‌تواند با میکروسرویس‌های مختلف ادغام شود:

- **User Service**: مدیریت کاربران و پروفایل‌ها
- **Order Service**: مدیریت سفارشات
- **Payment Service**: پردازش پرداخت‌ها
- **Notification Service**: ارسال اعلانات
- **Gateway**: API Gateway برای routing

## لاگ‌ها و Monitoring

لاگ‌ها با Serilog پیکربندی شده‌اند و شامل:
- Authentication events
- Authorization events
- Error tracking
- Performance metrics

## مجوز

این پروژه تحت مجوز Apache 2.0 منتشر شده است.
