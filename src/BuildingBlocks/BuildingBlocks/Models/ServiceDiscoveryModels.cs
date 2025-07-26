namespace BuildingBlocks.Models
{
    public class ServiceInstance
    {
        public string ServiceId { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public IReadOnlyList<string> Tags { get; set; } = Array.Empty<string>();
        public string BaseUrl => $"http://{Host}:{Port}";
    }

    public class ServiceRegistration
    {
        public string ServiceId { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Port { get; set; }
        public IReadOnlyList<string> Tags { get; set; } = Array.Empty<string>();
        public string HealthCheckEndpoint { get; set; } = "/health/live";
        public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromSeconds(10);
        public TimeSpan HealthCheckTimeout { get; set; } = TimeSpan.FromSeconds(5);
        public TimeSpan DeregisterCriticalServiceAfter { get; set; } = TimeSpan.FromMinutes(1);
    }

    public class ConsulSettings
    {
        public string Address { get; set; } = "http://localhost:8500";
    }
}