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
    /// Represents the SQL Server-specific implementation of the ChatDbContext.
    /// </summary>
    public class SqlServerChatDbContext : ChatDbContext, IChatDbContext
    {
        private string _connString;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerChatDbContext"/> class with the specified options.
        /// </summary>
        /// <param name="options">The options to be configured.</param>
        public SqlServerChatDbContext(DbContextOptions options) : this(options, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerChatDbContext"/> class with the specified options and connection string.
        /// </summary>
        /// <param name="options">The options to be configured.</param>
        /// <param name="connectionString">The connection string to be used for the database connection.</param>
        public SqlServerChatDbContext(DbContextOptions options, string connectionString) : base(options)
        {
            _connString = connectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerChatDbContext"/> class with the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to be used for the database connection.</param>
        public SqlServerChatDbContext(string connectionString)
        {
            _connString = connectionString;
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
            if (!string.IsNullOrEmpty(_connString))
                optionsBuilder.UseSqlServer(_connString, (o) =>
                {
                    o.MigrationsHistoryTable("__EFMigrationsHistory_BotMate", "botmate");
                });
        }
    }
}
