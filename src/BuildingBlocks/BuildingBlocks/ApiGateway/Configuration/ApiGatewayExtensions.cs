using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Builder;
using AspNetCoreRateLimit;
using BuildingBlocks.ApiGateway.Middleware;

namespace BuildingBlocks.ApiGateway.Configuration
{
    public static class ApiGatewayExtensions
    {
        public static IServiceCollection AddApiGateway(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Ocelot
            services.AddOcelot(configuration);

            // Add JWT Authentication
            var jwtSettings = configuration.GetSection("JWT").Get<JwtSettings>();
            if (jwtSettings != null)
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = jwtSettings.Issuer,
                            ValidAudience = jwtSettings.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                            ClockSkew = TimeSpan.Zero
                        };
                    });
            }

            // Add CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            services.AddOptions();
            // ذخیره شمارنده‌ها در حافظه توزیع‌شده (اینجا از حافظه داخلی استفاده شده، برای پروداکشن از Redis استفاده کنید)
            services.AddStackExchangeRedisCache(options =>
            // {
               options.Configuration = configuration.GetConnectionString("Redis");
            // });
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

            // ثبت سرویس‌های Rate Limiting
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            // برای Redis:
            services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();

            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
            services.AddInMemoryRateLimiting();

            return services;
        }

      

        public static IApplicationBuilder UseApiGateway(this IApplicationBuilder app)
        {
            app.UseIpRateLimiting();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseOcelot().Wait();

            return app;
        }
    }

    public class JwtSettings
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public int ExpirationMinutes { get; set; } = 60;
    }

    public class ApiGatewaySettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public int Port { get; set; } = 5000;
        public bool EnableRateLimiting { get; set; } = true;
        public int RateLimitRequestsPerMinute { get; set; } = 100;
        public bool EnableLogging { get; set; } = true;
    }
}
