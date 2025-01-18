using Luval.GenAIBotMate.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GenAIBotMate.Components.Infrastructure.Data
{
    /// <summary>
    /// Data transfer object for managing chat history.
    /// </summary>
    public class HistoryDto
    {
        /// <summary>
        /// Collection of chat sessions.
        /// </summary>
        public IQueryable<ChatSession> Sessions { get; set; } = default!;

        /// <summary>
        /// Indicates the ID of the session selected by the user
        /// </summary>
        public ulong? SessionId { get; set; } = default!;
        /// <summary>
        /// Indicates if the user has selected to delete a session
        /// </summary>
        public bool? IsDelete { get; set; } = default!;

    }
}
