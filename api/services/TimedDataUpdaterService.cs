using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SS.Api.services
{
    internal class TimedDataUpdaterService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        public IServiceProvider Services { get; }

        public TimedDataUpdaterService(IServiceProvider services, ILogger<TimedDataUpdaterService> logger)
        {
            Services = services;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");

            using var scope = Services.CreateScope();
            var justinDataUpdaterService =
                scope.ServiceProvider
                    .GetRequiredService<JustinDataUpdaterService>();

            /*await justinDataUpdaterService.SyncRegions();
            await justinDataUpdaterService.SyncLocations();
            await justinDataUpdaterService.SyncCourtRooms();*/
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
