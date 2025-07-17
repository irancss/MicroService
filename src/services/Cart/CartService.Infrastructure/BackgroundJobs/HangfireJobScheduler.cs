using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cart.Infrastructure.BackgroundJobs;

public class HangfireJobScheduler : IHostedService
{
    private readonly ILogger<HangfireJobScheduler> _logger;

    public HangfireJobScheduler(ILogger<HangfireJobScheduler> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Hangfire job scheduler");

        // Schedule recurring jobs
        RecurringJob.AddOrUpdate<CartAbandonmentJob>(
            "process-abandoned-carts",
            job => job.ProcessAbandonedCartsAsync(),
            "*/30 * * * *"); // Every 30 minutes

        RecurringJob.AddOrUpdate<CartAbandonmentJob>(
            "move-abandoned-to-next-purchase",
            job => job.MoveAbandonedCartsToNextPurchaseAsync(),
            "0 2 * * *"); // Daily at 2 AM

        RecurringJob.AddOrUpdate<CartAbandonmentJob>(
            "cleanup-expired-carts",
            job => job.CleanupExpiredCartsAsync(),
            "0 3 * * 0"); // Weekly on Sunday at 3 AM

        _logger.LogInformation("Hangfire jobs scheduled successfully");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Hangfire job scheduler");
        return Task.CompletedTask;
    }
}
