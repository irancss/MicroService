using BuildingBlocks.Resiliency;
using BuildingBlocks.ServiceMesh.Http;
using BuildingBlocks.ServiceMesh.LoadBalancing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.ServiceMesh
{
    public static class ServiceMeshExtensions
    {
        public static IServiceCollection AddServiceMeshHttpClient(this IServiceCollection services, IConfiguration configuration)
        {
            var loadBalancerType = configuration["ServiceMesh:LoadBalancer"] ?? "RoundRobin";
            switch (loadBalancerType.ToLowerInvariant())
            {
                case "random":
                    services.AddSingleton<ILoadBalancer, RandomLoadBalancer>();
                    break;
                case "smoothweightedroundrobin": // [جدید]
                    services.AddSingleton<ILoadBalancer, SmoothWeightedRoundRobinLoadBalancer>();
                    break;
                default:
                    services.AddSingleton<ILoadBalancer, RoundRobinLoadBalancer>();
                    break;
            }

            services.AddTransient<ServiceDiscoveryDelegatingHandler>();

            services.AddHttpClient<IServiceMeshHttpClient, ServiceMeshHttpClient>()
                .AddHttpMessageHandler<ServiceDiscoveryDelegatingHandler>()
                .AddResilientHttpPolicies(configuration);

            return services;
        }
    }
}