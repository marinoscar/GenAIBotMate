using Luval.GenAIBotMate.Core.Entities;
using Luval.GenAIBotMate.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Luval.GenAIBotMate.Sample
{
    /// <summary>
    /// Provides helper methods for database initialization and seeding.
    /// </summary>
    public static class DbHelper
    {
        /// <summary>
        /// Initializes the database and seeds initial data if necessary.
        /// </summary>
        /// <param name="context">The database context to use for initialization.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task InitializeAsync(IChatDbContext context, CancellationToken cancellationToken = default)
        {
            // Make sure the DB is created
            await context.Database.EnsureCreatedAsync(cancellationToken);

            // Add some data
            if (!(await context.GenAIBots.AnyAsync(cancellationToken)))
            {
                context.GenAIBots.Add(new GenAIBot
                {
                    Name = "Gen AI Bot",
                    UtcCreatedOn = DateTime.UtcNow,
                    UtcUpdatedOn = DateTime.UtcNow,
                    Version = 1
                });
                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
