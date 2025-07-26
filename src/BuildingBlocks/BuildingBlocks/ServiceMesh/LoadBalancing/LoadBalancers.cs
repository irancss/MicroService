using BuildingBlocks.Models;
using System.Collections.Concurrent;

namespace BuildingBlocks.ServiceMesh.LoadBalancing
{
    public interface ILoadBalancer
    {
        ServiceInstance? SelectInstance(IList<ServiceInstance> instances, string serviceName);
    }

    public class RoundRobinLoadBalancer : ILoadBalancer
    {
        private readonly ConcurrentDictionary<string, int> _counters = new();

        public ServiceInstance? SelectInstance(IList<ServiceInstance> instances, string serviceName)
        {
            if (instances == null || !instances.Any())
                return null;

            // Get the next index in a thread-safe manner
            var index = _counters.AddOrUpdate(serviceName, 1, (key, value) => (value + 1) % instances.Count);
            return instances[index];
        }
    }

    public class RandomLoadBalancer : ILoadBalancer
    {
        public ServiceInstance? SelectInstance(IList<ServiceInstance> instances, string serviceName)
        {
            if (instances == null || !instances.Any())
                return null;

            var index = Random.Shared.Next(instances.Count);
            return instances[index];
        }
    }

    /// <summary>
    /// [جدید] یک پیاده‌سازی آماده برای پروداکشن از الگوریتم Smooth Weighted Round-Robin.
    /// این الگوریتم توزیع بار را به صورت بسیار نرم و دقیق بر اساس وزن سرورها انجام می‌دهد.
    /// </summary>
    public class SmoothWeightedRoundRobinLoadBalancer : ILoadBalancer
    {
        private readonly ConcurrentDictionary<string, List<ServerState>> _serverStates = new();
        private readonly object _lock = new();

        public ServiceInstance? SelectInstance(IList<ServiceInstance> instances, string serviceName)
        {
            if (instances == null || !instances.Any())
            {
                return null;
            }

            lock (_lock)
            {
                // اگر لیست سرورها برای این سرویس تغییر کرده (مثلا یک نمونه اضافه/حذف شده)، لیست state را بازسازی می‌کنیم
                if (!_serverStates.ContainsKey(serviceName) || _serverStates[serviceName].Count != instances.Count)
                {
                    var newStates = instances.Select(inst => new ServerState
                    {
                        Instance = inst,
                        EffectiveWeight = GetWeightFromTags(inst),
                        CurrentWeight = 0 // مقدار اولیه CurrentWeight صفر است
                    }).ToList();
                    _serverStates[serviceName] = newStates;
                }

                var states = _serverStates[serviceName];
                if (!states.Any()) return null;

                ServerState? bestServer = null;
                var totalWeight = states.Sum(s => s.EffectiveWeight);

                // الگوریتم Smooth Weighted Round-Robin
                foreach (var state in states)
                {
                    state.CurrentWeight += state.EffectiveWeight;
                    if (bestServer == null || state.CurrentWeight > bestServer.CurrentWeight)
                    {
                        bestServer = state;
                    }
                }

                if (bestServer == null) return null; // نباید اتفاق بیفتد مگر اینکه همه وزن‌ها صفر باشند

                bestServer.CurrentWeight -= totalWeight;

                return bestServer.Instance;
            }
        }

        private static int GetWeightFromTags(ServiceInstance instance)
        {
            var weightTag = instance.Tags?.FirstOrDefault(t => t.StartsWith("weight=", StringComparison.OrdinalIgnoreCase));
            if (weightTag != null && int.TryParse(weightTag.Split('=')[1], out var weight))
            {
                return weight > 0 ? weight : 1;
            }
            return 1; // وزن پیش‌فرض
        }

        private class ServerState
        {
            public ServiceInstance Instance { get; set; } = null!;
            public int EffectiveWeight { get; set; }
            public int CurrentWeight { get; set; }
        }
    }
}