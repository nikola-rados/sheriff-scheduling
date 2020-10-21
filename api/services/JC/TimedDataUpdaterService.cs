using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SS.Api.Helpers;
using SS.Api.services.JC;

namespace SS.Api.services
{
    internal class TimedDataUpdaterService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        public IServiceProvider Services { get; }
        private readonly TimeSpan _jcSynchronizationPeriod;

        public TimedDataUpdaterService(IServiceProvider services, ILogger<TimedDataUpdaterService> logger, IConfiguration configuration)
        {
            Services = services;
            _logger = logger;
            _jcSynchronizationPeriod = TimeSpan.Parse(configuration.GetNonEmptyValue("JCSynchronization:Period"));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Timed Background Service is starting with a period of {_jcSynchronizationPeriod}.");
            _timer = new Timer(DoWork, null, new TimeSpan(), _jcSynchronizationPeriod);
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            try
            {
                _logger.LogInformation("Timed Background Service is working.");

                using var scope = Services.CreateScope();
                var justinDataUpdaterService =
                    scope.ServiceProvider
                        .GetRequiredService<JCDataUpdaterService>();

                _logger.LogInformation("Syncing Regions.");
                await justinDataUpdaterService.SyncRegions();
                _logger.LogInformation("Syncing Locations.");
                await justinDataUpdaterService.SyncLocations();
                _logger.LogInformation("Syncing CourtRooms.");
                await justinDataUpdaterService.SyncCourtRooms();
                _logger.LogInformation("Finished Syncing.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error happened while syncing regions/locations/courtrooms.");
            }
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
