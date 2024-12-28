using Luval.GenAIBotMate.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GenAIBotMate.Components.Infrastructure.Data
{
    public class HistoryDto
    {
        public IQueryable<ChatSession> Sessions { get; set; } = default!;
    }
}
