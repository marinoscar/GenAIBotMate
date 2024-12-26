using Luval.GenAIBotMate.Core.Entities;
using Luval.GenAIBotMate.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GenAIBotMate.Tests.Helpers
{
    public class MemoryDataContext : ChatDbContext
    {
        public MemoryDataContext() : base(GetOptions())
        {
        }

        private static DbContextOptions GetOptions()
        {
            var options = new DbContextOptionsBuilder<MemoryDataContext>()
                        .UseSqlite("Filename=:memory:") // Use in-memory SQLite database
                        .LogTo((s) => Debug.WriteLine(s))
                        .EnableSensitiveDataLogging()
                        .Options;
            return options;
        }

        public void Initialize()
        {
            Database.OpenConnection();
            Database.EnsureCreated();

            var chatbot = new GenAIBot()
            {
                AccountId = 1,
                Name = "Test Chatbot",
                SystemPrompt = "Test System Prompt",
                Version = 1,
                UtcCreatedOn = DateTime.UtcNow,
                UtcUpdatedOn = DateTime.UtcNow
            };

            GenAIBots.Add(chatbot);
            SaveChanges();
        }

        public override void Dispose()
        {
            Database.EnsureDeleted();
            base.Dispose();
        }
    }
}
