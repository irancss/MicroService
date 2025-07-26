using Microsoft.Extensions.Configuration;
using Winton.Extensions.Configuration.Consul;

namespace BuildingBlocks.Configuration
{
    /// <summary>
    /// [اصلاح شد] این کلاس ساده‌سازی شده و فقط بر روی قابلیت اصلی تمرکز دارد.
    /// Provides extension methods for adding Consul as a centralized configuration source.
    /// </summary>
    public static class CentralizedConfigurationExtensions
    {
        /// <summary>
        /// Adds Consul as a configuration provider to the configuration builder.
        /// This allows the application to load settings from Consul's Key/Value store.
        /// </summary>
        public static IConfigurationBuilder AddConsulConfiguration(this IConfigurationBuilder builder, IConfiguration configuration)
        {
            var consulAddress = configuration["Consul:Address"];
            if (string.IsNullOrEmpty(consulAddress))
            {
                return builder;
            }

            var serviceName = configuration["Service:Name"] ?? "unknown-service";
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            // [نکته] کلید در کنسول به این شکل خواهد بود: "configurations/service-name/environment"
            // این ساختار به سازماندهی بهتر کانفیگ‌ها کمک می‌کند.
            var consulKey = $"configurations/{serviceName}/{environment}";

            return builder.AddConsul(
                consulKey,
                options =>
                {
                    options.ConsulConfigurationOptions = consulOptions =>
                    {
                        consulOptions.Address = new Uri(consulAddress);
                    };
                    options.Optional = true;
                    options.ReloadOnChange = true;
                    options.OnLoadException = exceptionContext =>
                    {
                        Console.WriteLine($"Failed to load configuration from Consul for key '{consulKey}'. Error: {exceptionContext.Exception.Message}");
                        exceptionContext.Ignore = true;
                    };
                });
        }
    }
}