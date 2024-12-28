using Luval.GenAIBotMate.Core.Entities;
using Luval.GenAIBotMate.Core.Services;
using Luval.GenAIBotMate.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GenAIBotMate.Components
{
    public partial class GenAIBotMateControl : ComponentBase
    {

        private string userMessage = "";
        private string agentStreamMessage = "";

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Parameter]
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

        [Parameter]
        public string InitialMessage { get; set; } = "Hello! How can I help you today?";

        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public bool IsStreaming { get; set; } = false;

        [Parameter]
        public string ChatTitle { get; set; } = "New Chat";

        [Parameter]
        public required Func<ChatMessageResult, Task> SubmitClicked { get; set; }

        [Parameter]
        public string? GenAIChatbotName { get; set; }

        [Inject]
        public required GenAIBotService Service {get;set;}

        [Inject]
        public required IGenAIBotStorageService StorageService { get; set; }


        internal GenAIBot Bot { get; private set; }

        
        
        protected virtual async Task OnSubmitClickedAsync()
        {

            await SubmitClicked?.Invoke(new ChatMessageResult()
            {
                UserMessage = userMessage,
                History = Messages
            });

            StateHasChanged();
            Debug.WriteLine("Message Count: {0}", Messages.Count);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                await JSRuntime.InvokeVoidAsync("window.scrollToBottom");
            await base.OnAfterRenderAsync(firstRender);
        }


        protected virtual async Task InitializeControlAsync(CancellationToken cancellationToken = default)
        {
            Service.ChatMessageCompleted += ChatMessageCompletedAsync;
            Service.ChatMessageStream += ChatMessageStreamAsync;
            if (Bot == null)
            {
                Bot = await StorageService.GetChatbotAsync(GenAIChatbotName, cancellationToken).ConfigureAwait(false);
                if(Bot == null)
                {
                    Bot = await StorageService.GetChatbotAsync(1, cancellationToken).ConfigureAwait(false);
                }
                if(Bot == null) throw new InvalidOperationException($"There are no instance of {nameof(GenAIBot)} in the {nameof(IGenAIBotStorageService)} and it is required");
            }
        }

        private async Task ChatMessageStreamAsync(ChatMessageStreamResult result)
        {
            throw new NotImplementedException();
        }

        private async Task ChatMessageCompletedAsync(ChatMessageCompletedResult result)
        {
            throw new NotImplementedException();
        }
    }

    public class ChatMessageResult : EventArgs
    {
        public string? UserMessage { get; set; }
        public List<ChatMessage>? History { get; set; }
    }
}
