using Luval.GenAIBotMate.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Luval.GenAIBotMate.Infrastructure.Interfaces
{
    /// <summary>
    /// The interface for the ChatDbContext.
    /// </summary>
    public interface IChatDbContext
    {
        /// <summary>
        /// The DbSet representing the GenAIBot table.
        /// </summary>
        DbSet<GenAIBot> GenAIBots { get; set; }

        /// <summary>
        /// The DbSet representing the ChatMessage table.
        /// </summary>
        DbSet<ChatMessage> ChatMessages { get; set; }

        /// <summary>
        /// The DbSet representing the ChatSession table.
        /// </summary>
        DbSet<ChatSession> ChatSessions { get; set; }

        /// <summary>
        /// The DbSet representing the ChatMessageMedia table.
        /// </summary>
        DbSet<ChatMessageMedia> ChatMessageMedia { get; set; }

        /// <summary>
        /// The database instance for the context.
        /// </summary>
        public DatabaseFacade Database { get; }

        /// <summary>
        /// The ChangeTracker for the context.
        /// </summary>
        public ChangeTracker ChangeTracker { get; }

        /// <summary>
        /// Saves the changes made to the context.
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// Saves the changes made to the context asynchronously.
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}