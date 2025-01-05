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
        private readonly IChatDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenAIBotContextHelper"/> class with the specified logger.
        /// </summary>
        /// <param name="context">The database context to use for initialization.</param>
        /// <param name="logger">The logger to use for logging operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when the logger is null.</exception>
        public GenAIBotContextHelper(IChatDbContext context, ILogger<GenAIBotContextHelper> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenAIBotContextHelper"/> class with a default logger.
        /// </summary>
        /// <param name="context">The database context to use for initialization.</param>
        public GenAIBotContextHelper(IChatDbContext context) : this(context, new ColorConsoleLogger<GenAIBotContextHelper>())
        {
        }

        /// <summary>
        /// Initializes the database context by ensuring the database is created, applying any pending migrations, and seeding initial data.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <remarks>
        /// This method performs the following actions:
        /// 1. Checks if the database can be connected to. If not, it ensures the database is created.
        /// 2. If the database can be connected to, it checks for any pending migrations and applies them if necessary.
        /// 3. Seeds the database with initial data if the GenAIBots table is empty.
        /// </remarks>
        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Initializing database context...");
            // Make sure the DB is created
            if (!await _context.Database.CanConnectAsync())
            {
                await CreateDatabaseAsync(cancellationToken);
            }
            if (!CheckForTables())
            {
                await CreateDatabaseAsync(cancellationToken);
            }
            if (!(await _context.GenAIBots.AnyAsync(cancellationToken)))
            {
                // Add some data
                _logger.LogInformation("Seeding initial data...");
                _context.GenAIBots.Add(new GenAIBot
                {
                    Name = "Gen AI Bot",
                    UtcCreatedOn = DateTime.UtcNow,
                    CreatedBy = "System",
                    UtcUpdatedOn = DateTime.UtcNow,
                    UpdatedBy = "System",
                    Version = 1
                });
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Initial data seeded.");
            }
        }

        private async Task CreateDatabaseAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Database does not exist. Running first migration...");
            var createScript = _context.Database.GenerateCreateScript();
            await _context.Database.ExecuteSqlRawAsync(createScript, cancellationToken);
        }

        private bool CheckForTables(CancellationToken cancellationToken = default)
        {
            try
            {
                // Check for the GenAIBots table
                _context.GenAIBots.Select(c => c.Id).Take(1).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Schema not created.");
                return false;
            }
            return true;
        }
    }
}
