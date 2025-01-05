using Luval.GenAIBotMate.Core.Entities;
using Luval.GenAIBotMate.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GenAIBotMate.Infrastructure.Data
{
    /// <summary>
    /// Provides helper methods for database initialization and seeding.
    /// </summary>
    public class GenAIBotContextHelper
    {
        
        /// <summary>
        /// Initializes the database context by ensuring the database is created, applying any pending migrations, and seeding initial data.
        /// </summary>
        /// <param name="context">The database context to initialize.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <remarks>
        /// This method performs the following actions:
        /// 1. Checks if the database can be connected to. If not, it ensures the database is created.
        /// 2. If the database can be connected to, it checks for any pending migrations and applies them if necessary.
        /// 3. Seeds the database with initial data if the GenAIBots table is empty.
        /// </remarks>
        public static async Task InitializeAsync(IChatDbContext context, CancellationToken cancellationToken = default)
        {
            // Make sure the DB is created
            if (!await context.Database.CanConnectAsync())
                await context.Database.EnsureCreatedAsync();
            else
            {
                // Migrate if necessary
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);
                if (pendingMigrations.Any())
                    await context.Database.MigrateAsync(cancellationToken);
            }

            // Add some data
            if (!(await context.GenAIBots.AnyAsync(cancellationToken)))
            {
                context.GenAIBots.Add(new GenAIBot
                {
                    Name = "Gen AI Bot",
                    UtcCreatedOn = DateTime.UtcNow,
                    CreatedBy = "System",
                    UtcUpdatedOn = DateTime.UtcNow,
                    UpdatedBy = "System",
                    Version = 1
                });
                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
