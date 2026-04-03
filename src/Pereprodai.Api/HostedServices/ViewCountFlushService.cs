using Pereprodai.Catalog.Application.Services;

namespace Pereprodai.Api.HostedServices;

public class ViewCountFlushService: BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ViewCountFlushService> _logger;

    public ViewCountFlushService(IServiceScopeFactory scopeFactory, ILogger<ViewCountFlushService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(5));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await FlushViewCounts();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error flushing view counts");
            }
        }
    }

    private async Task FlushViewCounts()
    {
        using var scope = _scopeFactory.CreateScope();
        var viewCountService = scope.ServiceProvider.GetRequiredService<IViewCountService>();
        await viewCountService.FlushViewCountsAsync();
    }
}