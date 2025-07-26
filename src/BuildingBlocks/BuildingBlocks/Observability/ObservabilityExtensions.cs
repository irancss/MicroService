using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using Npgsql;


namespace BuildingBlocks.Observability
{
    public static class ObservabilityExtensions
    {
        public static IServiceCollection AddObservability(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceName = configuration["Service:Name"] ?? "unknown-service";
            services.Configure<ObservabilitySettings>(configuration.GetSection("Observability"));
            var observabilitySettings = configuration.GetSection("Observability").Get<ObservabilitySettings>() ?? new ObservabilitySettings();
            observabilitySettings.ServiceName = serviceName;

            AddOpenTelemetry(services, observabilitySettings, configuration);
            services.AddSingleton<ICustomMetrics, CustomMetrics>();

            return services;
        }

        private static void AddOpenTelemetry(IServiceCollection services, ObservabilitySettings settings, IConfiguration configuration)
        {
            var resourceBuilder = ResourceBuilder.CreateDefault()
                .AddService(settings.ServiceName, serviceVersion: settings.ServiceVersion)
                .AddTelemetrySdk()
                .AddEnvironmentVariableDetector();

            // [مهم] Instrumentation برای Redis باید اینجا به Services اضافه شود.
            var dbTypeForRedis = configuration.GetConnectionString("CacheDatabaseType")?.ToLowerInvariant();
            if (dbTypeForRedis == "redis")
            {
              //  services.AddStackExchangeRedisInstrumentation();
            }

            services.AddOpenTelemetry()
                .WithMetrics(builder =>
                {
                    builder
                        .SetResourceBuilder(resourceBuilder)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddMeter(settings.ServiceName)
                        .AddPrometheusExporter();
                })
                .WithTracing(builder =>
                {
                    builder
                        .SetResourceBuilder(resourceBuilder)
                        .SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(0.2)))
                        .AddAspNetCoreInstrumentation(options => options.RecordException = true)
                        .AddHttpClientInstrumentation(options => options.RecordException = true)
                        .AddSource(settings.ServiceName)
                        .AddSource("MassTransit");

                    // بر اساس نوع دیتابیس، Instrumentation مناسب را اضافه می‌کنیم.
                    AddDatabaseInstrumentation(builder, configuration);

                    if (settings.EnableJaegerTracing && !string.IsNullOrEmpty(settings.JaegerHost))
                    {
                        builder.AddJaegerExporter(o =>
                        {
                            o.AgentHost = settings.JaegerHost;
                            o.AgentPort = settings.JaegerPort;
                        });
                    }
                });
        }

        private static void AddDatabaseInstrumentation(TracerProviderBuilder builder, IConfiguration configuration)
        {
            // خواندن نوع دیتابیس از ConnectionStrings
            var dbType = configuration.GetConnectionString("DatabaseType")?.ToLowerInvariant();

            // خواندن نوع دیتابیس کش از یک کلید دیگر برای جلوگیری از تداخل
            var cacheDbType = configuration.GetConnectionString("CacheDatabaseType")?.ToLowerInvariant();

            switch (dbType)
            {
                case "sqlserver":
                    builder.AddSqlClientInstrumentation(options =>
                    {
                        options.SetDbStatementForText = true;
                        options.RecordException = true;
                    });
                    break;

                case "postgres":
                    builder.AddNpgsql();
                    break;

                case "mongodb":
                    builder.AddMongoDBInstrumentation();
                    break;

                //case "cassandra":
                //    builder.AddCassandraInstrumentation(options =>
                //    {
                //        // گزینه‌های کمتری دارد اما خطاها را ثبت می‌کند.
                //        options.RecordException = true;
                //    });
                //    break;
                //case "elasticsearch":
                //    builder.AddElasticsearchClientInstrumentation(options =>
                //    {
                //        // گزینه‌هایی برای کنترل جزئیات ثبت درخواست‌ها و پاسخ‌ها دارد.
                //        options.RecordException = true;
                //    });
                //    break;
            }

        }

        public static IHostBuilder AddSerilogLogging(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseSerilog((context, services, loggerConfiguration) =>
            {
                var serviceName = context.Configuration["Service:Name"] ?? "unknown-service";
                var seqUrl = context.Configuration["Observability:SeqUrl"];

                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("ServiceName", serviceName)
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .Enrich.WithSpan()
                    .WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} <s:{ServiceName} t:{TraceId}> {Properties:j}{NewLine}{Exception}",
                        theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code);

                if (!string.IsNullOrEmpty(seqUrl))
                {
                    loggerConfiguration.WriteTo.Seq(seqUrl);
                }
            });
        }

        public static IApplicationBuilder UseObservability(this IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging();
            app.UseOpenTelemetryPrometheusScrapingEndpoint();
            return app;
        }
    }

    public class ObservabilitySettings
    {
        public string ServiceName { get; set; } = "unknown-service";
        public string ServiceVersion { get; set; } = "1.0.0";
        public bool EnableJaegerTracing { get; set; } = true;
        public string JaegerHost { get; set; } = "localhost";
        public int JaegerPort { get; set; } = 6831;
    }

    public interface ICustomMetrics
    {
        void IncrementCounter(string name, params KeyValuePair<string, object?>[] tags);
        void RecordHistogram(string name, double value, params KeyValuePair<string, object?>[] tags);
        IDisposable MeasureDuration(string name, params KeyValuePair<string, object?>[] tags);
    }

    public class CustomMetrics : ICustomMetrics
    {
        private readonly Meter _meter;
        private readonly ConcurrentDictionary<string, Counter<long>> _counters = new();
        private readonly ConcurrentDictionary<string, Histogram<double>> _histograms = new();

        public CustomMetrics(IConfiguration configuration)
        {
            var serviceName = configuration["Service:Name"] ?? "unknown-service";
            _meter = new Meter(serviceName);
        }

        public void IncrementCounter(string name, params KeyValuePair<string, object?>[] tags)
        {
            var counter = _counters.GetOrAdd(name, (n) => _meter.CreateCounter<long>(n));
            counter.Add(1, tags);
        }

        public void RecordHistogram(string name, double value, params KeyValuePair<string, object?>[] tags)
        {
            var histogram = _histograms.GetOrAdd(name, (n) => _meter.CreateHistogram<double>(n));
            histogram.Record(value, tags);
        }

        public IDisposable MeasureDuration(string name, params KeyValuePair<string, object?>[] tags)
        {
            return new DurationMeasurement(this, name, tags);
        }

        private class DurationMeasurement : IDisposable
        {
            private readonly CustomMetrics _metrics;
            private readonly string _name;
            private readonly KeyValuePair<string, object?>[] _tags;
            private readonly Stopwatch _stopwatch;

            public DurationMeasurement(CustomMetrics metrics, string name, KeyValuePair<string, object?>[] tags)
            {
                _metrics = metrics;
                _name = name;
                _tags = tags;
                _stopwatch = Stopwatch.StartNew();
            }

            public void Dispose()
            {
                _stopwatch.Stop();
                _metrics.RecordHistogram($"{_name}_duration_ms", _stopwatch.Elapsed.TotalMilliseconds, _tags);
            }
        }
    }
}