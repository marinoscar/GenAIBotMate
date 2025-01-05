using Luval.AuthMate.Infrastructure.Logging;
using Luval.GenAIBotMate.Core.Entities;
using Luval.GenAIBotMate.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<GenAIBotContextHelper> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenAIBotContextHelper"/> class with the specified logger.
        /// </summary>
        /// <param name="logger">The logger to use for logging operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when the logger is null.</exception>
        public GenAIBotContextHelper(ILogger<GenAIBotContextHelper> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenAIBotContextHelper"/> class with a default logger.
        /// </summary>
        public GenAIBotContextHelper() : this(new ColorConsoleLogger<GenAIBotContextHelper>())
        {
        }

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
        public async Task InitializeAsync(IChatDbContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Initializing database context...");
            // Make sure the DB is created
            if (!await context.Database.CanConnectAsync())
            {
                _logger.LogInformation("Database does not exist. Creating database...");
                await context.Database.EnsureCreatedAsync();
            }
            else
            {
                _logger.LogInformation("Database exists. Checking for pending migrations...");
                // Migrate if necessary
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);
                if (pendingMigrations.Any())
                {
                    _logger.LogInformation("Applying pending migrations...");
                    await context.Database.MigrateAsync(cancellationToken);
                }
                else
                {
                    _logger.LogInformation("No pending migrations found.");
                }

                // Add some data
                _logger.LogInformation("Seeding initial data...");
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
                    _logger.LogInformation("Initial data seeded.");
                }
            }
        }
    }
}
