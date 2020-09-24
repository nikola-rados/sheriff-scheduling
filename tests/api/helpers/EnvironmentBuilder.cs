using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using SS.Api.Helpers;
using SS.Api.Helpers.Extensions;
using SS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using SS.Api.Models.DB;
using SS.Db.models;

namespace tests.api.Helpers
{
    /// <summary>
    /// This builds out our environment before injecting into controllers.
    /// </summary>
    public class EnvironmentBuilder
    {
        public ILoggerFactory LogFactory;
        public HttpClient HttpClient;
        public IConfiguration Configuration;

        public EnvironmentBuilder(string usernameKey, string passwordKey, string urlKey)
        {
            //Load in secrets, this uses the secrets file as the API.
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: true);
            builder.AddUserSecrets<EnvironmentBuilder>();
            Configuration = builder.Build();

            //Create HTTP client, usually done by Startup.cs - which handles the life cycle of HttpClient nicely.
            HttpClient = new HttpClient();

            HttpClient.DefaultRequestHeaders.Authorization = new SS.Api.Models.BasicAuthenticationHeaderValue(
                Configuration.GetNonEmptyValue(usernameKey),
                Configuration.GetNonEmptyValue(passwordKey));

            HttpClient.BaseAddress = new Uri(Configuration.GetNonEmptyValue(urlKey).EnsureEndingForwardSlash());
            //Create logger.
            LogFactory = LoggerFactory.Create(loggingBuilder =>
            {
                loggingBuilder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddConsole();
            });
        }

        public static DbContextOptions<SheriffDbContext> SetupDbOptions(bool useMemoryDatabase = false)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: true);
            builder.AddUserSecrets<EnvironmentBuilder>();
            var configuration = builder.Build();

            var optionsBuilder = new DbContextOptionsBuilder<SheriffDbContext>();
            if (useMemoryDatabase)
                optionsBuilder.UseInMemoryDatabase("testingDb");
            else
                optionsBuilder.UseNpgsql(configuration.GetNonEmptyValue("ConnectionStrings.DB")).EnableSensitiveDataLogging(true);

            return optionsBuilder.Options;
        }

    }

}