using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.Identity;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = configuration["IdentityServer:Authority"];
                options.Audience = configuration["IdentityServer:Audience"];

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    options.RequireHttpsMetadata = false;
                }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidAudience = configuration["IdentityServer:Audience"],
                    ValidIssuer = configuration["IdentityServer:Authority"]
                };
            });

        // افزودن پشتیبانی از Authorization Policy ها
        services.AddAuthorization(options =>
        {
            // این Policy به عنوان پیش‌فرض برای تمام endpoint هایی که نیاز به احراز هویت دارند، استفاده می‌شود.
            options.AddPolicy("default", policy => policy.RequireAuthenticatedUser());

            options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
            options.AddPolicy("VipUser", policy => policy.RequireClaim("IsVip", "true"));
        });

        return services;
    }
}