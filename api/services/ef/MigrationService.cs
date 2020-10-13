using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SS.Db.models;

namespace SS.Api.services.ef
{
    public class MigrationService
    {
        public IServiceProvider Services { get; }
        private readonly ILogger<MigrationService> _logger;

        public MigrationService(IServiceProvider services, ILogger<MigrationService> logger)
        {
            Services = services;
            _logger = logger;
        }

        public void ExecuteMigrations()
        {
            try
            {
                _logger.LogInformation("Starting Migrations.");
                using var scope = Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<SheriffDbContext>();
                db.Database.Migrate();
                _logger.LogInformation("Migrations complete.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Database migration failed on startup.");
            }
        }
    }
}
