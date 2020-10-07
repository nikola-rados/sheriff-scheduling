using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SS.Db.models;

namespace SS.Api.infrastructure
{
    /// <summary>
    /// ApplicationBuilderExtensions static class, provides extension methods for ApplicationBuilder objects.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Initialize the database when the application starts.
        /// This isn't an ideal way to do this, but will work for our purposes.
        /// </summary>
        /// <param name="app"></param>
        public static void UpdateDatabase<T>(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();
            var logger = serviceScope.ServiceProvider.GetService<ILogger<T>>();

            try
            {
                using var context = serviceScope.ServiceProvider.GetService<SheriffDbContext>();
                context.Database.Migrate();
                logger.LogInformation("Migrations complete.");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Database migration failed on startup.");
            }
        }
    }
}
