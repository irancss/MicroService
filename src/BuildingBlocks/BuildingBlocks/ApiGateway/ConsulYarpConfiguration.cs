using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace BuildingBlocks.ApiGateway
{
public class ConsulProxyConfig : IProxyConfig
{
    private readonly CancellationTokenSource _cts = new();

    public IReadOnlyList<RouteConfig> Routes { get; }
    public IReadOnlyList<ClusterConfig> Clusters { get; }
    public IChangeToken ChangeToken { get; }

    public ConsulProxyConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
    {
        Routes = routes;
        Clusters = clusters;
        ChangeToken = new CancellationChangeToken(_cts.Token);
    }

    internal void SignalChange()
    {
        _cts.Cancel();
    }
}

// این کلاس وظیفه ارائه کانفیگ به YARP را دارد و اجازه می‌دهد از بیرون آپدیت شود
public class InMemoryConfigProvider : IProxyConfigProvider
{
    private volatile ConsulProxyConfig _config;

    public InMemoryConfigProvider()
    {
        // با یک کانفیگ خالی شروع می‌کنیم تا در زمان استارت‌آپ خطایی رخ ندهد
        _config = new ConsulProxyConfig(new List<RouteConfig>(), new List<ClusterConfig>());
    }

    public IProxyConfig GetConfig() => _config;

    public void Update(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
    {
        var oldConfig = _config;
        _config = new ConsulProxyConfig(routes, clusters);
        // به کانفیگ قدیمی سیگنال می‌دهیم تا YARP متوجه تغییر شود و GetConfig() را دوباره فراخوانی کند
        oldConfig.SignalChange();
    }
}
}