using AspNetCoreRateLimit;
using BuildingBlocks.ServiceMesh.Http; // [جدید] برای تزریق IServiceMeshHttpClient
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Yarp.ReverseProxy.Configuration;

namespace BuildingBlocks.ApiGateway
{
    /// <summary>
    /// [اصلاح شد] فایل اصلی برای پیکربندی کامل و یکپارچه YARP API Gateway.
    /// </summary>
    public static class YarpApiGatewayExtensions
    {
        public static IServiceCollection AddYarpApiGateway(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Configure YARP Routes from appsettings.json
            services.Configure<YarpRoutesConfig>(configuration.GetSection("YarpRoutes"));

            // 2. Add YARP and configure it to load routes/clusters dynamically from our custom provider.
            services.AddReverseProxy().LoadFromConsul();

            // 3. Add JWT Authentication that integrates with an Identity Provider.
            var authority = configuration["IdentityServer:Authority"];
            if (!string.IsNullOrEmpty(authority))
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.Authority = authority;
                        options.RequireHttpsMetadata = false; // Set to true in production
                        options.Audience = configuration["IdentityServer:Audience"];
                    });
            }
            services.AddAuthorization(options =>
            {
                options.AddPolicy("default", policy => policy.RequireAuthenticatedUser());
            });

            // 4. Add Aggregation services.
            // [اصلاح شد] تزریق IServiceMeshHttpClient برای استفاده در AggregationService
            services.AddScoped<IAggregationService, AggregationService>();

            // 5. Add a secure CORS policy.
            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            services.AddCors(options =>
            {
                options.AddPolicy("DefaultCorsPolicy", policy =>
                {
                    if (allowedOrigins.Any())
                    {
                        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                    }
                    else
                    {
                        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); // Fallback for development
                    }
                });
            });

            // 6. Add Rate Limiting.
            services.AddOptions();
            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
            services.AddInMemoryRateLimiting();

            return services;
        }

        public static IApplicationBuilder UseYarpApiGateway(this IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseIpRateLimiting();
            app.UseCors("DefaultCorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Map aggregation endpoints to be handled directly by the Gateway.
                endpoints.MapGet("/api/aggregated/dashboard/{userId}", async (string userId, IAggregationService service) =>
                {
                    var data = await service.GetDashboardDataAsync(userId);
                    return data != null ? Results.Ok(data) : Results.NotFound();
                }).RequireAuthorization("default");

                endpoints.MapGet("/api/aggregated/order/{orderId}", async (int orderId, IAggregationService service) =>
                {
                    var data = await service.GetOrderDetailsAsync(orderId);
                    return data != null ? Results.Ok(data) : Results.NotFound();
                }).RequireAuthorization("default");

                // Map YARP as the final endpoint to catch all other routes.
                endpoints.MapReverseProxy();
            });

            return app;
        }

        private static IReverseProxyBuilder LoadFromConsul(this IReverseProxyBuilder builder)
        {
            builder.Services.AddSingleton<IHostedService, ConsulConfigUpdaterService>();
            builder.Services.AddSingleton<InMemoryConfigProvider>();
            builder.Services.AddSingleton<IProxyConfigProvider>(sp => sp.GetRequiredService<InMemoryConfigProvider>());
            return builder;
        }
    }
}