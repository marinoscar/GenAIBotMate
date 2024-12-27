using Luval.GenAIBotMate.Core.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GenAIBotMate.Components
{
    public partial class GenAIBotMateControl : ComponentBase
    {

        private string userMessage = "";
        private string agentStreamMessage = "";

        [Parameter]
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

        [Parameter]
        public string InitialMessage { get; set; } = "Hello! How can I help you today?";

        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public bool IsStreaming { get; set; } = false;



        public event EventHandler<ChatMessageEventArgs>? SubmitClicked;

        public virtual void AddAgentText(string text)
        {
            agentStreamMessage += text;
            StateHasChanged();
        }

        protected virtual void OnSubmitClicked()
        {
            SubmitClicked?.Invoke(this, new ChatMessageEventArgs()
            {
                UserMessage = userMessage,
                History = Messages
            });
            StateHasChanged();
            Debug.WriteLine("Message Count: {0}", Messages.Count);
        }

    }

    public class ChatMessageEventArgs : EventArgs
    {
        public string? UserMessage { get; set; }
        public List<ChatMessage>? History { get; set; }
    }
}
