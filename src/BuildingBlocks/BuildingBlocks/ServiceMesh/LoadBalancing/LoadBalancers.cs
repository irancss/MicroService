using BuildingBlocks.ServiceDiscovery;

namespace BuildingBlocks.ServiceMesh.LoadBalancing
{
    public interface ILoadBalancer
    {
        ServiceInstance? SelectService(IEnumerable<ServiceInstance> services);
    }

    public class RoundRobinLoadBalancer : ILoadBalancer
    {

         private static readonly ConcurrentDictionary<string, int> _counters = new();

        public ServiceInstance? SelectService(IEnumerable<ServiceInstance> services)
        {
            var serviceList = services.Where(s => s.IsHealthy).ToList();

            if (serviceList == null || !serviceList.Any())
                return null;

            if (serviceList.Count == 1)
                return serviceList.First();

            var serviceName = serviceList.First().ServiceName;

            // Get the current counter value for the service, and increment it atomically.
            var counter = _counters.AddOrUpdate(serviceName, 1, (key, value) => value + 1);

            // Get the index of the service to select.
            var index = (counter - 1) % serviceList.Count;

            return serviceList[index];
        }
    }

    public class RandomLoadBalancer : ILoadBalancer
    {
        private readonly Random _random = new();

        public ServiceInstance? SelectService(IEnumerable<ServiceInstance> services)
        {
            var serviceList = services.Where(s => s.IsHealthy).ToList();
            
            if (!serviceList.Any())
                return null;

            var index = _random.Next(serviceList.Count);
            return serviceList[index];
        }
    }

    public class WeightedRoundRobinLoadBalancer : ILoadBalancer
    {
        private readonly Dictionary<string, (int currentWeight, int totalWeight)> _weights = new();
        private readonly object _lock = new object();

        public ServiceInstance? SelectService(IEnumerable<ServiceInstance> services)
        {
            var serviceList = services.Where(s => s.IsHealthy).ToList();
            
            if (!serviceList.Any())
                return null;

            if (serviceList.Count == 1)
                return serviceList.First();

            lock (_lock)
            {
                var serviceName = serviceList.First().ServiceName;
                
                if (!_weights.ContainsKey(serviceName))
                {
                    _weights[serviceName] = (0, serviceList.Count);
                }

                // Simple weighted round-robin implementation
                var (currentWeight, totalWeight) = _weights[serviceName];
                currentWeight = (currentWeight + 1) % totalWeight;
                _weights[serviceName] = (currentWeight, totalWeight);

                return serviceList[currentWeight];
            }
        }
    }
}
