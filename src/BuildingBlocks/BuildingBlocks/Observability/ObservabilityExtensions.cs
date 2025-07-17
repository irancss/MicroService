using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Prometheus;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace BuildingBlocks.Observability
{
    /// <summary>
    /// Comprehensive observability with distributed tracing, structured logging, and metrics
    /// </summary>
    public static class ObservabilityExtensions
    {
        public static IServiceCollection AddObservability(this IServiceCollection services, IConfiguration configuration, string serviceName)
        {
            var observabilitySettings = configuration.GetSection("Observability").Get<ObservabilitySettings>() ?? new ObservabilitySettings();
            services.Configure<ObservabilitySettings>(configuration.GetSection("Observability"));

            // Add OpenTelemetry
            AddOpenTelemetry(services, observabilitySettings, serviceName);

            // Add custom metrics
            AddCustomMetrics(services, serviceName);

            // Add performance monitoring
            services.AddSingleton<IPerformanceMonitor, PerformanceMonitor>();

            return services;
        }

        public static IHostBuilder AddSerilogLogging(this IHostBuilder hostBuilder, IConfiguration configuration)
        {
            return hostBuilder.UseSerilog((context, loggerConfiguration) =>
            {
                var observabilitySettings = configuration.GetSection("Observability").Get<ObservabilitySettings>() ?? new ObservabilitySettings();

                loggerConfiguration
                    .MinimumLevel.Is(Enum.Parse<LogEventLevel>(observabilitySettings.LogLevel))
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("ServiceName", observabilitySettings.ServiceName)
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                    .WriteTo.Seq(observabilitySettings.SeqUrl, apiKey: observabilitySettings.SeqApiKey);

                if (observabilitySettings.EnableFileLogging)
                {
                    loggerConfiguration.WriteTo.File(
                        path: observabilitySettings.LogFilePath,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 30);
                }
            });
        }

        public static IApplicationBuilder UseObservability(this IApplicationBuilder app)
        {
            // Add Prometheus metrics endpoint
            app.UseMetricServer("/metrics");

            // Add HTTP metrics middleware
            app.UseHttpMetrics();

            return app;
        }

        private static void AddOpenTelemetry(IServiceCollection services, ObservabilitySettings settings, string serviceName)
        {
            services.AddOpenTelemetry()
                .ConfigureResource(resource => 
                    resource.AddService(serviceName, settings.ServiceVersion)
                            .AddAttributes(new Dictionary<string, object>
                            {
                                ["service.environment"] = settings.Environment,
                                ["service.instance.id"] = Environment.MachineName
                            }))
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation(options =>
                        {
                            options.RecordException = true;
                            options.Filter = context => 
                                !context.Request.Path.Value?.Contains("/health") == true &&
                                !context.Request.Path.Value?.Contains("/metrics") == true;
                        })
                        .AddHttpClientInstrumentation(options =>
                        {
                            options.RecordException = true;
                        })
                        .AddSource(serviceName);

                    if (settings.EnableJaegerTracing)
                    {
                        tracing.AddJaegerExporter(options =>
                        {
                            options.AgentHost = settings.JaegerHost;
                            options.AgentPort = settings.JaegerPort;
                        });
                    }
                })
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddMeter(serviceName);
                });
        }

        private static void AddCustomMetrics(IServiceCollection services, string serviceName)
        {
            services.AddSingleton<ActivitySource>(provider => new ActivitySource(serviceName));
            services.AddSingleton<Meter>(provider => new Meter(serviceName));
            services.AddSingleton<ICustomMetrics, CustomMetrics>();
        }
    }

    public class ObservabilitySettings
    {
        public string ServiceName { get; set; } = "unknown-service";
        public string ServiceVersion { get; set; } = "1.0.0";
        public string Environment { get; set; } = "Development";
        public string LogLevel { get; set; } = "Information";
        public bool EnableJaegerTracing { get; set; } = true;
        public string JaegerHost { get; set; } = "localhost";
        public int JaegerPort { get; set; } = 14268;
        public bool EnablePrometheusMetrics { get; set; } = true;
        public string SeqUrl { get; set; } = "http://localhost:5341";
        public string SeqApiKey { get; set; } = "";
        public bool EnableFileLogging { get; set; } = false;
        public string LogFilePath { get; set; } = "logs/service-.log";
    }

    public interface ICustomMetrics
    {
        void IncrementCounter(string name, params KeyValuePair<string, object?>[] tags);
        void RecordValue(string name, double value, params KeyValuePair<string, object?>[] tags);
        void SetGauge(string name, double value, params KeyValuePair<string, object?>[] tags);
        IDisposable StartTimer(string name, params KeyValuePair<string, object?>[] tags);
    }

    public class CustomMetrics : ICustomMetrics
    {
        private readonly Meter _meter;
        private readonly Dictionary<string, Counter<long>> _counters = new();
        private readonly Dictionary<string, Histogram<double>> _histograms = new();
        private readonly Dictionary<string, ObservableGauge<double>> _gauges = new();

        public CustomMetrics(Meter meter)
        {
            _meter = meter;
        }

        public void IncrementCounter(string name, params KeyValuePair<string, object?>[] tags)
        {
            if (!_counters.TryGetValue(name, out var counter))
            {
                counter = _meter.CreateCounter<long>(name);
                _counters[name] = counter;
            }

            counter.Add(1, tags);
        }

        public void RecordValue(string name, double value, params KeyValuePair<string, object?>[] tags)
        {
            if (!_histograms.TryGetValue(name, out var histogram))
            {
                histogram = _meter.CreateHistogram<double>(name);
                _histograms[name] = histogram;
            }

            histogram.Record(value, tags);
        }

        public void SetGauge(string name, double value, params KeyValuePair<string, object?>[] tags)
        {
            // Note: Observable gauges are more complex to implement with dynamic values
            // This is a simplified implementation
            var gaugeName = $"{name}_gauge";
            if (!_gauges.ContainsKey(gaugeName))
            {
                _gauges[gaugeName] = _meter.CreateObservableGauge<double>(gaugeName, () => value);
            }
        }

        public IDisposable StartTimer(string name, params KeyValuePair<string, object?>[] tags)
        {
            return new TimerScope(this, name, tags);
        }

        private class TimerScope : IDisposable
        {
            private readonly CustomMetrics _metrics;
            private readonly string _name;
            private readonly KeyValuePair<string, object?>[] _tags;
            private readonly Stopwatch _stopwatch;

            public TimerScope(CustomMetrics metrics, string name, KeyValuePair<string, object?>[] tags)
            {
                _metrics = metrics;
                _name = name;
                _tags = tags;
                _stopwatch = Stopwatch.StartNew();
            }

            public void Dispose()
            {
                _stopwatch.Stop();
                _metrics.RecordValue($"{_name}_duration_ms", _stopwatch.ElapsedMilliseconds, _tags);
            }
        }
    }

    public interface IPerformanceMonitor
    {
        Task<PerformanceMetrics> GetCurrentMetricsAsync();
        void RecordOperation(string operationName, TimeSpan duration, bool success = true);
    }

    public class PerformanceMonitor : IPerformanceMonitor
    {
        private readonly ICustomMetrics _customMetrics;
        private readonly ILogger<PerformanceMonitor> _logger;

        public PerformanceMonitor(ICustomMetrics customMetrics, ILogger<PerformanceMonitor> logger)
        {
            _customMetrics = customMetrics;
            _logger = logger;
        }

        public async Task<PerformanceMetrics> GetCurrentMetricsAsync()
        {
            await Task.Delay(1); // Simulate async operation

            var process = Process.GetCurrentProcess();
            
            return new PerformanceMetrics
            {
                CpuUsagePercent = GetCpuUsage(),
                MemoryUsageMB = process.WorkingSet64 / 1024 / 1024,
                ThreadCount = process.Threads.Count,
                GCCollectionCount = GC.CollectionCount(0) + GC.CollectionCount(1) + GC.CollectionCount(2),
                Timestamp = DateTimeOffset.UtcNow
            };
        }

        public void RecordOperation(string operationName, TimeSpan duration, bool success = true)
        {
            var tags = new KeyValuePair<string, object?>[]
            {
                new("operation", operationName),
                new("success", success)
            };

            _customMetrics.RecordValue("operation_duration_ms", duration.TotalMilliseconds, tags);
            _customMetrics.IncrementCounter("operation_count", tags);

            if (!success)
            {
                _customMetrics.IncrementCounter("operation_failures", 
                    new KeyValuePair<string, object?>("operation", operationName));
            }

            _logger.LogInformation("Operation {OperationName} completed in {Duration}ms (Success: {Success})",
                operationName, duration.TotalMilliseconds, success);
        }

        private double GetCpuUsage()
        {
            // This is a simplified CPU usage calculation
            // In production, you might want to use more sophisticated approaches
            var process = Process.GetCurrentProcess();
            return process.TotalProcessorTime.TotalMilliseconds / Environment.ProcessorCount;
        }
    }

    public class PerformanceMetrics
    {
        public double CpuUsagePercent { get; set; }
        public long MemoryUsageMB { get; set; }
        public int ThreadCount { get; set; }
        public int GCCollectionCount { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// Extension methods for adding tracing to operations
    /// </summary>
    public static class TracingExtensions
    {
        public static async Task<T> TraceAsync<T>(this Task<T> task, ActivitySource activitySource, string operationName, 
            IDictionary<string, object>? tags = null)
        {
            using var activity = activitySource.StartActivity(operationName);
            
            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    activity?.SetTag(tag.Key, tag.Value);
                }
            }

            try
            {
                var result = await task;
                activity?.SetStatus(ActivityStatusCode.Ok);
                return result;
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                activity?.SetTag("exception.type", ex.GetType().Name);
                activity?.SetTag("exception.message", ex.Message);
                throw;
            }
        }

        public static T Trace<T>(this Func<T> func, ActivitySource activitySource, string operationName,
            IDictionary<string, object>? tags = null)
        {
            using var activity = activitySource.StartActivity(operationName);
            
            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    activity?.SetTag(tag.Key, tag.Value);
                }
            }

            try
            {
                var result = func();
                activity?.SetStatus(ActivityStatusCode.Ok);
                return result;
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                activity?.SetTag("exception.type", ex.GetType().Name);
                activity?.SetTag("exception.message", ex.Message);
                throw;
            }
        }
    }
}
