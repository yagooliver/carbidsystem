
using CarBidSystem.Auctions.UseCases.Auctions.Commands;
using CarBidSystem.Common.Configurations;
using MediatR;
using Microsoft.Extensions.Options;

namespace CarBidSystem.Auctions.Service.Services
{
    public class EndExpiredAuctionsService(
        IServiceProvider serviceProvider,
        ILogger<StartUpcomingAuctionsService> logger,
        IOptions<BackgroundServiceConfig> config) : BackgroundService
    {
        private readonly int executionLimit = config.Value.ExecutionIntervalInMilliseconds;

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("EndExpiredAuctionsService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("EndExpiredAuctionsService is running at: {time}", DateTimeOffset.Now);
                using var scope = serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Send(new EndExpiredAuctionsCommand(), stoppingToken);

                await Task.Delay(executionLimit, stoppingToken);
            }

            logger.LogInformation("EndExpiredAuctionsService is stopping.");
        }
    }
}
