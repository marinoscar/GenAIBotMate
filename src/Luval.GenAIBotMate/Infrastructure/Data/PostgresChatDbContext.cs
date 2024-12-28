using Luval.AuthMate.Core.Entities;
using Luval.GenAIBotMate.Core.Entities;
using Luval.GenAIBotMate.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GenAIBotMate.Infrastructure.Data
{
    /// <summary>
    /// Postgres implementation of the database context.
    /// </summary>
    public class PostgresChatDbContext : ChatDbContext, IChatDbContext
    {

        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgresChatDbContext"/> class with the specified options.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public PostgresChatDbContext(DbContextOptions<PostgresChatDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgresChatDbContext"/> class with the specified options and connection string.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        /// <param name="connectionString">The connection string to the PostgreSQL database.</param>
        public PostgresChatDbContext(DbContextOptions<PostgresChatDbContext> options, string connectionString) : base(options)
        {
            _connectionString = connectionString ?? throw new ArgumentException(nameof(connectionString));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgresChatDbContext"/> class with the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to the PostgreSQL database.</param>
        public PostgresChatDbContext(string connectionString) : base(new DbContextOptionsBuilder<ChatDbContext>().UseNpgsql(connectionString).Options)
        {
            _connectionString = connectionString ?? throw new ArgumentException(nameof(connectionString));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //continue with regular implementation
            base.OnConfiguring(optionsBuilder);


            //add conn string if provided
            if (!string.IsNullOrEmpty(_connectionString))
                optionsBuilder.UseNpgsql(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GenAIBot>()
               .Property(at => at.Id)
               .UseIdentityColumn()
               .HasColumnType("BIGINT");

            modelBuilder.Entity<ChatSession>()
               .Property(at => at.Id)
               .UseIdentityColumn()
               .HasColumnType("BIGINT");

            modelBuilder.Entity<ChatMessage>()
               .Property(at => at.Id)
               .UseIdentityColumn()
               .HasColumnType("BIGINT");

            modelBuilder.Entity<ChatMessageMedia>()
               .Property(at => at.Id)
               .UseIdentityColumn()
               .HasColumnType("BIGINT");

        }
    }
}
