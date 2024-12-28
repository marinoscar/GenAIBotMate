using Luval.GenAIBotMate.Components.Infrastructure.Configuration;
using Luval.GenAIBotMate.Core.Entities;
using Luval.GenAIBotMate.Core.Services;
using Luval.GenAIBotMate.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.GenAIBotMate.Components
{
    public partial class GenAIBotMateControl : ComponentBase
    {

        private string userMessage = "";
        private string agentStreamMessage = "";


        [Parameter]
        public string InitialMessage { get; set; } = "Hello! How can I help you today?";


        [Parameter]
        public string ChatTitle { get; set; } = "New Chat";

        [Parameter]
        public required Func<ChatMessageResult, Task> SubmitClicked { get; set; }

        [Parameter]
        public string? GenAIChatbotName { get; set; }

        [Parameter]
        public GenAIBotOptions Options { get; set; } = new GenAIBotOptions();

        [Inject]
        public required GenAIBotService Service { get; set; }

        [Inject]
        public required IGenAIBotStorageService StorageService { get; set; }

        [Inject]
        public required IJSRuntime JSRuntime { get; set; }


        /// <summary>
        /// The GenAIBot instance to be used for the chat.
        /// </summary>
        internal GenAIBot Bot { get; private set; } = default!;

        /// <summary>
        /// The ChatMessage instance to be streamed.
        /// </summary>
        internal ChatMessage StreamedMessage { get; private set; } = default!;

        /// <summary>
        /// The ChatSession is loading
        /// </summary>
        protected bool IsLoading { get; set; } = false;

        /// <summary>
        /// The ChatSession instance to be streamed.
        /// </summary>
        protected bool IsStreaming { get; set; } = false;

        /// <summary>
        /// The messages to be rendered on the screen.
        /// </summary>
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();



        protected virtual async Task OnSubmitClickedAsync()
        {
            try
            {
                IsLoading = true;
                IsStreaming = false;
                var settings = new OpenAIPromptExecutionSettings()
                {
                    Temperature = Options.Temperature,
                    ModelId = Options.Model
                };

                var firstMessage = StreamedMessage == null;
                // Add the in-progress message to be rendered on the screen
                StreamedMessage = new ChatMessage()
                {
                    Id = 9999,
                    UserMessage = userMessage,
                    AgentResponse = "Loading..."
                };
                Messages.Add(StreamedMessage);
                StateHasChanged(); // Update the UI to show the loading message

                StreamedMessage = firstMessage
                    ? await Service.SubmitMessageToNewSession(Bot.Id, userMessage, settings: settings).ConfigureAwait(false)
                    : await Service.AppendMessageToSession(userMessage, StreamedMessage.ChatSessionId, settings: settings);

                IsLoading = false;
                IsStreaming = false;

                Messages.Clear();
                Messages.AddRange(StreamedMessage.ChatSession.ChatMessages);
                if (Messages.Count == 1)
                {
                    await UpdateSessionTitleBasedOnHistoryAsync();
                }

                StateHasChanged();
                Debug.WriteLine("Message Count: {0}", Messages.Count);
            }
            catch (Exception ex)
            {
                IsLoading = false;
                IsStreaming = false;
                Debug.WriteLine("Error in OnSubmitClickedAsync: {0}", ex.Message);
                throw new InvalidOperationException("Error in OnSubmitClickedAsync", ex);
            }
        }

        protected override  async Task OnInitializedAsync()
        {
            Service.ChatMessageCompleted += ChatMessageCompletedAsync;
            Service.ChatMessageStream += ChatMessageStreamAsync;

            Bot = await GetBotAsync(CancellationToken.None);
            if (Bot == null)
            {
                throw new InvalidOperationException($"There are no instance of {nameof(GenAIBot)} in the {nameof(IGenAIBotStorageService)} and it is required");
            }
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                await JSRuntime.InvokeVoidAsync("window.scrollToBottom");
        }



        public async Task UpdateSessionTitleBasedOnHistoryAsync()
        {
            await UpdateSessionTitleAsync(await GetChatTitleAsync());
        }

        public async Task UpdateSessionTitleAsync(string title)
        {
            StreamedMessage.ChatSession.Title = title;
            await StorageService.UpdateChatSessionAsync(StreamedMessage.ChatSession);
        }

        private async Task<string> GetChatTitleAsync(CancellationToken cancellationToken = default)
        {
            var prompt = @"
From the following conversation, please extract a title for the chat.
Here is the conversation:

{body}

- Keep the title to less than 80 characters 
- Just return the title on a single line and nothing else.

";
            var body = new StringBuilder();
            foreach (var msg in StreamedMessage.ChatSession.ChatMessages)
            {
                body.AppendFormat("User Message: {0}\n\nAgent Response: {1}\n", msg.UserMessage, msg.AgentResponse);
            }
            var settings = new OpenAIPromptExecutionSettings()
            {
                Temperature = 0.8,
                ModelId = "gpt-3.5-turbo"
            };
            var res = await Service.GetChatMessageAsync(prompt.Replace("{body}", body.ToString()), settings);
            var content = res.Items.Where(i => i is TextContent).Select(i => i as TextContent).ToList();
            return string.Join(Environment.NewLine, content);
        }


        private async Task<GenAIBot?> GetBotAsync(CancellationToken cancellationToken)
        {
            if (Bot != null) return Bot;
            Debug.WriteLine("Getting bot from storage", "IMPORTANT");
            var bot = await StorageService.GetChatbotAsync(GenAIChatbotName, cancellationToken).ConfigureAwait(false);
            return bot ?? await StorageService.GetChatbotAsync(1, cancellationToken).ConfigureAwait(false);
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
