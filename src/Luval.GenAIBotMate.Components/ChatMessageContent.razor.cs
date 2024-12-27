using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GenAIBotMate.Components
{
    public partial class ChatMessageContent : ComponentBase
    {
        [Parameter]
        public string AgentResponse { get; set; } = "";

        [Parameter]
        public string UserMessage { get; set; } = "";
    }
}
