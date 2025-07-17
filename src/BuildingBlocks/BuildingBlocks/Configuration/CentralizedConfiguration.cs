using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Winton.Extensions.Configuration.Consul;

namespace BuildingBlocks.Configuration
{
    /// <summary>
    /// Centralized configuration using Consul KV store
    /// </summary>
    public static class CentralizedConfigurationExtensions
    {
        public static IConfigurationBuilder AddConsulConfiguration(
            this IConfigurationBuilder builder,
            string consulAddress = "http://localhost:8500",
            string serviceName = "default",
            string environment = "Development")
        {
            return builder.AddConsul(
                $"{serviceName}/{environment}",
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
                        // Log the exception but don't fail startup
                        Console.WriteLine($"Failed to load configuration from Consul: {exceptionContext.Exception?.Message}");
                        exceptionContext.Ignore = true;
                    };
                });
        }

        public static IServiceCollection AddCentralizedConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Register configuration change monitoring
            services.AddSingleton<IConfigurationChangeMonitor, ConfigurationChangeMonitor>();
            services.AddHostedService<ConfigurationRefreshService>();

            return services;
        }
    }

    public interface IConfigurationChangeMonitor
    {
        event Action<string, object?> ConfigurationChanged;
        void StartMonitoring();
        void StopMonitoring();
    }

    public class ConfigurationChangeMonitor : IConfigurationChangeMonitor
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConfigurationChangeMonitor> _logger;
        private readonly Dictionary<string, object?> _lastKnownValues = new();
        private IDisposable? _changeToken;

        public event Action<string, object?>? ConfigurationChanged;

        public ConfigurationChangeMonitor(IConfiguration configuration, ILogger<ConfigurationChangeMonitor> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void StartMonitoring()
        {
            _changeToken = _configuration.GetReloadToken().RegisterChangeCallback(OnConfigurationChanged, null);
            
            // Store initial values
            StoreCurrentValues();
            
            _logger.LogInformation("Configuration change monitoring started");
        }

        public void StopMonitoring()
        {
            _changeToken?.Dispose();
            _logger.LogInformation("Configuration change monitoring stopped");
        }

        private void OnConfigurationChanged(object? state)
        {
            _logger.LogInformation("Configuration change detected, checking for specific changes...");
            
            CheckForChanges();
            
            // Re-register for the next change
            _changeToken = _configuration.GetReloadToken().RegisterChangeCallback(OnConfigurationChanged, null);
        }

        private void CheckForChanges()
        {
            var currentValues = GetCurrentValues();
            
            foreach (var kvp in currentValues)
            {
                if (_lastKnownValues.TryGetValue(kvp.Key, out var lastValue))
                {
                    if (!Equals(lastValue, kvp.Value))
                    {
                        _logger.LogInformation("Configuration key '{Key}' changed from '{OldValue}' to '{NewValue}'",
                            kvp.Key, lastValue, kvp.Value);
                        
                        ConfigurationChanged?.Invoke(kvp.Key, kvp.Value);
                    }
                }
                else
                {
                    _logger.LogInformation("New configuration key '{Key}' added with value '{Value}'",
                        kvp.Key, kvp.Value);
                    
                    ConfigurationChanged?.Invoke(kvp.Key, kvp.Value);
                }
            }
            
            // Check for removed keys
            foreach (var kvp in _lastKnownValues)
            {
                if (!currentValues.ContainsKey(kvp.Key))
                {
                    _logger.LogInformation("Configuration key '{Key}' was removed", kvp.Key);
                    ConfigurationChanged?.Invoke(kvp.Key, null);
                }
            }
            
            StoreCurrentValues();
        }

        private void StoreCurrentValues()
        {
            _lastKnownValues.Clear();
            var currentValues = GetCurrentValues();
            foreach (var kvp in currentValues)
            {
                _lastKnownValues[kvp.Key] = kvp.Value;
            }
        }

        private Dictionary<string, object?> GetCurrentValues()
        {
            var values = new Dictionary<string, object?>();
            
            // Get all configuration keys from all providers
            var configRoot = _configuration as IConfigurationRoot;
            if (configRoot != null)
            {
                foreach (var provider in configRoot.Providers)
                {
                    // Check if this is a Consul provider by name
                    if (provider.GetType().Name.Contains("Consul"))
                    {
                        var childKeys = provider.GetChildKeys(Enumerable.Empty<string>(), null);
                        foreach (var key in childKeys)
                        {
                            if (provider.TryGet(key, out var value))
                            {
                                values[key] = value;
                            }
                        }
                    }
                }
            }
            
            return values;
        }
    }

    public class ConfigurationRefreshService : BackgroundService
    {
        private readonly IConfigurationChangeMonitor _monitor;
        private readonly ILogger<ConfigurationRefreshService> _logger;

        public ConfigurationRefreshService(
            IConfigurationChangeMonitor monitor,
            ILogger<ConfigurationRefreshService> logger)
        {
            _monitor = monitor;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _monitor.StartMonitoring();
            
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                    // The monitoring is event-driven, so we just keep the service alive
                }
            }
            finally
            {
                _monitor.StopMonitoring();
            }
        }
    }

    /// <summary>
    /// Sample strongly-typed configuration class
    /// </summary>
    public class ServiceConfiguration
    {
        public string ServiceName { get; set; } = string.Empty;
        public string Version { get; set; } = "1.0.0";
        public DatabaseConfiguration Database { get; set; } = new();
        public MessageBrokerConfiguration MessageBroker { get; set; } = new();
        public LoggingConfiguration Logging { get; set; } = new();
    }

    public class DatabaseConfiguration
    {
        public string ConnectionString { get; set; } = string.Empty;
        public int CommandTimeout { get; set; } = 30;
        public bool EnableRetryOnFailure { get; set; } = true;
    }

    public class MessageBrokerConfiguration
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
    }

    public class LoggingConfiguration
    {
        public string MinimumLevel { get; set; } = "Information";
        public bool EnableStructuredLogging { get; set; } = true;
        public string SeqServerUrl { get; set; } = "http://localhost:5341";
    }
}
