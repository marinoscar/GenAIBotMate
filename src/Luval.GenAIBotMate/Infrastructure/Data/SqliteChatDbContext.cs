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
    /// Represents the SQLite database context for managing chat-related entities.
    /// </summary>
    public class SqliteChatDbContext : ChatDbContext, IChatDbContext
    {
        private readonly string? _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteChatDbContext"/> class with the default connection string.
        /// </summary>
        public SqliteChatDbContext() : this("Data Source=app.db")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteChatDbContext"/> class with the specified options.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public SqliteChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteChatDbContext"/> class with the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by the DbContext.</param>
        public SqliteChatDbContext(string connectionString) : base(new DbContextOptionsBuilder<ChatDbContext>().UseSqlite(connectionString).Options)
        {
            _connectionString = connectionString ?? throw new ArgumentException(nameof(connectionString));
        }

        /// <summary>
        /// Configures the database context options.
        /// </summary>
        /// <param name="optionsBuilder">The options builder used to configure the context.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Continue with regular implementation
            base.OnConfiguring(optionsBuilder);

            // Add connection string if provided
            if (!string.IsNullOrEmpty(_connectionString))
                optionsBuilder.UseSqlite(_connectionString, (o) => {
                    o.MigrationsHistoryTable("__EFMigrationsHistory_BotMate", "botmate");
                });
        }
    }
}
