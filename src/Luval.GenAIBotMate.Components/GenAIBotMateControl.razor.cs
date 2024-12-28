using Luval.GenAIBotMate.Components.Infrastructure.Configuration;
using Luval.GenAIBotMate.Components.Infrastructure.Data;
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

        private string _userMessage = "";
        private string _agentStreamMessage = "";
        private ulong _activeSessionId = 0;
        private IDialogReference? _historyDialog;


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

        [Inject]
        public required IDialogService DialogService { get; set; }


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


        
        ///<summary>
        /// Handles the submission of a user message, processes it using the GenAIBotService, and updates the UI accordingly.
        /// </summary>
        /// <remarks>
        /// This method performs the following steps:
        /// 1. Sets the loading and streaming states to true and false, respectively.
        /// 2. Configures the OpenAIPromptExecutionSettings with the temperature and model ID from the Options property.
        /// 3. Checks if this is the first message in the session.
        /// 4. Creates a new ChatMessage instance with a loading indicator and adds it to the Messages list.
        /// 5. Updates the UI to show the loading message.
        /// 6. Submits the user message to a new session or appends it to an existing session using the GenAIBotService.
        /// 7. Updates the loading and streaming states to false.
        /// 8. Clears the Messages list and repopulates it with the chat messages from the current session.
        /// 9. If this is the first message in the session, updates the session title based on the chat history.
        /// 10. Updates the UI to reflect the new messages.
        /// 11. Logs the number of messages in the session.
        /// 12. Catches any exceptions, logs the error, and rethrows an InvalidOperationException.
        /// </remarks>
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
                    ChatSessionId = _activeSessionId,
                    UserMessage = _userMessage
                };
                Messages.Add(StreamedMessage);

                await InvokeAsync(StateHasChanged); // Update the UI to show the loading message

                StreamedMessage = firstMessage
                    ? await Service.SubmitMessageToNewSession(Bot.Id, _userMessage, settings: settings).ConfigureAwait(false)
                    : await Service.AppendMessageToSession(_userMessage, StreamedMessage.ChatSessionId, settings: settings);

                _activeSessionId = StreamedMessage.ChatSessionId; //keep track of the session id
                IsLoading = false;
                IsStreaming = false;

                Messages.Clear();
                Messages.AddRange(StreamedMessage.ChatSession.ChatMessages);
                if (Messages.Count == 1)
                {
                    await UpdateSessionTitleBasedOnHistoryAsync();
                }

                //clear the user message
                _userMessage = "";
                await InvokeAsync(StateHasChanged); // Update the UI to show the loading message
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

        /// <summary>
        /// Loads a chat session asynchronously and updates the UI with the session's messages.
        /// </summary>
        /// <param name="session">The chat session to load.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <remarks>
        /// This method performs the following steps:
        /// 1. Validates the session parameter to ensure it is not null and has a valid ID.
        /// 2. Clears the current user message and sets the chat session ID.
        /// 3. Clears the existing messages and adds the messages from the provided session.
        /// 4. Sets the streamed message to the last message in the session.
        /// 5. Invokes StateHasChanged to update the UI.
        /// </remarks>
        protected virtual async Task LoadSessionAsync(ChatSession session, CancellationToken cancellationToken = default)
        {
            if (session == null) return;
            if (session.Id <= 0) return;
            //gets the full history
            var fullSession = await StorageService.GetChatSessionAsync(session.Id, cancellationToken);

            if (fullSession.ChatMessages == null || !fullSession.ChatMessages.Any()) return;
            ChatTitle = fullSession.Title;
            _userMessage = "";
            _activeSessionId = fullSession.Id;
            Messages.Clear();
            Messages.AddRange(fullSession.ChatMessages);
            StreamedMessage = fullSession.ChatMessages.Last();
            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Starts a new chat session by clearing the current user message, resetting the active session ID, and clearing the message list.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual async Task StartNewChatAsync()
        {
            _userMessage = "";
            _activeSessionId = 0;
            StreamedMessage = default!;
            ChatTitle = "New Chat";
            Messages.Clear();
            await InvokeAsync(StateHasChanged);
        }


        /// <summary>
        /// Displays the chat history in a side panel.
        /// </summary>
        /// <remarks>
        /// This method performs the following steps:
        /// 1. Retrieves the chat sessions for the current bot, ordered by the last updated date, limited to 20 sessions.
        /// 2. Creates a HistoryDto object containing the retrieved sessions and the delete and navigate functions.
        /// 3. Displays the chat history in a side panel using the DialogService.
        /// 4. Waits for the user to close the dialog and captures the result.
        /// </remarks>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual async Task ShowHistoryAsync()
        {
            var sessions = StorageService.GetChatSessions(Bot.Id, null, (i => i.UtcUpdatedOn), false, 20);
            var history = new HistoryDto()
            {
                Sessions = sessions,
                DeleteFunction = HandleDelete,
                NavigateFunction = HandleNavigation
            };

            _historyDialog = await DialogService.ShowPanelAsync<MessageHistory>(history, new DialogParameters<HistoryDto>()
            {
                Content = history,
                Alignment = HorizontalAlignment.Right,
                Title = $"Chat History",
                PrimaryAction = "Close",
                SecondaryActionEnabled = false,
                SecondaryAction = null
            });

            var result = await _historyDialog.Result;
        }

        protected async Task HandleDelete(ChatSession session)
        {
            if (session == null) return;
            if (session.Id <= 0) return;
            await StorageService.DeleteChatSessionAsync(session.Id);
        }

        protected async Task HandleNavigation(ChatSession session)
        {
            await LoadSessionAsync(session);
            await _historyDialog?.CloseAsync();
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

- Keep the title to just a few short words 
- Just return the title on a single line and nothing else.
- Title not exceed 80 characters

";
            var body = new StringBuilder();
            foreach (var msg in StreamedMessage.ChatSession.ChatMessages)
            {
                body.AppendFormat("User Message: {0}\n\nAgent Response: {1}\n", msg.UserMessage, msg.AgentResponse);
            }
            var settings = new OpenAIPromptExecutionSettings()
            {
                Temperature = 0.8,
                ModelId = OpenAIModels.GPT35_Turbo
            };
            var res = await Service.GetChatMessageAsync(prompt.Replace("{body}", body.ToString()), settings);
            var content = res.Items.Where(i => i is TextContent).Select(i => i as TextContent).ToList();
            return string.Join(Environment.NewLine, content);
        }


        private async Task<GenAIBot?> GetBotAsync(CancellationToken cancellationToken)
        {
            if (Bot != null) return Bot;
            var bot = await StorageService.GetChatbotAsync(GenAIChatbotName, cancellationToken).ConfigureAwait(false);
            return bot ?? await StorageService.GetChatbotAsync(1, cancellationToken).ConfigureAwait(false);
        }

        private async Task ChatMessageStreamAsync(ChatMessageStreamResult result)
        {
            IsLoading = false;
            IsStreaming = true;
            if (StreamedMessage != null)
            {
                StreamedMessage.AgentResponse += result.Content; //append the message from the AI
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task ChatMessageCompletedAsync(ChatMessageCompletedResult result)
        {
            IsLoading = false;
            IsStreaming = false;
            if (StreamedMessage != null)
            {
                StreamedMessage.AgentResponse += result.Content; //append the message from the AI
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    public class ChatMessageResult
    {
        public string? UserMessage { get; set; }
        public List<ChatMessage>? History { get; set; }
    }
}
