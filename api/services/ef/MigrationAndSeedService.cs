using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Hosting;
using SS.Db.models;

namespace SS.Api.services.ef
{
    public class MigrationAndSeedService
    {
        public IServiceProvider Services { get; }
        private readonly ILogger<MigrationAndSeedService> _logger;

        public MigrationAndSeedService(IServiceProvider services, ILogger<MigrationAndSeedService> logger)
        {
            Services = services;
            _logger = logger;
        }

        public void ExecuteMigrationsAndSeeds()
        {
            try
            {
                _logger.LogInformation("Starting Migrations.");
                using var scope = Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<SheriffDbContext>();
                var environment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
                var migrationsAssembly = db.GetService<IMigrationsAssembly>();
                var historyRepository = db.GetService<IHistoryRepository>();

                var all = migrationsAssembly.Migrations.Keys;
                var applied = historyRepository.GetAppliedMigrations().Select(r => r.MigrationId).ToList();
                var pending = all.Except(applied).ToList();

                _logger.LogInformation($"Pending {pending.Count} Migrations.");
                _logger.LogDebug($"{string.Join(", ", pending)}");
                db.Database.Migrate();
                _logger.LogInformation("Migration(s) complete.");

                if (applied.Count != 0) return;
                
                ExecuteSeedScripts(db, environment);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Database migration failed on startup.");
            }
        }

        private void ExecuteSeedScripts(SheriffDbContext db, IWebHostEnvironment environment)
        {
            var seedPath = environment.IsDevelopment() ? Path.Combine("docker", "seed") : "data";
            var dbSqlPath = environment.IsDevelopment() ? Path.Combine("db", "sql") : Path.Combine("src", "db", "sql");
            var path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).FullName, seedPath);
            var dbPath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).FullName, dbSqlPath);
            _logger.LogInformation($"Fresh database detected. Loading SQL from paths: {dbPath} and then {path}");

            var transaction = db.Database.BeginTransaction();
            var lastFile = "";
            try
            {
                var files = GetSqlFilesOrderedByNumber(dbPath).Concat(GetSqlFilesOrderedByNumber(path)).ToList();
                _logger.LogInformation($"Found {files.Count} files.");
                foreach (var file in files)
                {
                    lastFile = file;
                    _logger.LogInformation($"Executing File: {file}");
                    db.Database.ExecuteSqlRaw(File.ReadAllText(file));
                }
                transaction.Commit();
                _logger.LogInformation($"Executing files successful.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while executing {lastFile}. Rolling back all files.");
                transaction.Rollback();
            }
        }

        private IEnumerable<string> GetSqlFilesOrderedByNumber(string path)
        {
            if (Directory.Exists(path))
                return Directory.GetFiles(path, "*.sql").OrderBy(x =>
                    Regex.Match(x, @"\d+").Value);

            _logger.LogWarning($"{path} does not exist.");
            return new List<string>();
        }
    }
}
