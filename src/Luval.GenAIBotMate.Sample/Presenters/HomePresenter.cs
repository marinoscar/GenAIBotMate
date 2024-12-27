using Luval.GenAIBotMate.Components;
using Luval.GenAIBotMate.Core.Entities;
using Luval.GenAIBotMate.Core.Services;
using Luval.GenAIBotMate.Infrastructure.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Diagnostics;
using System.Text;

namespace Luval.GenAIBotMate.Sample.Presenters
{
    public class HomePresenter
    {

        private readonly GenAIBotService _service;
        private readonly IGenAIBotStorageService _storageService;
        private GenAIBotMateControl _control;
        private ChatMessage _lastMessage = default!;
        private GenAIBot _bot = default!;

        public event EventHandler UpdateState;

        /// <summary>
        /// Gets or sets the chat messages.
        /// </summary>
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
        public bool IsStreaming { get; set; } = false;

        public bool IsLoading { get; set; } = false;

        public string ChatTitle { get { return _lastMessage == null ? "New Chat" : _lastMessage.ChatSession?.Title ?? "New Chat"; } }

        public HomePresenter(GenAIBotService service, IGenAIBotStorageService storageService)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        public async Task InitializeAsync(GenAIBotMateControl control, CancellationToken cancellationToken = default)
        {
            _control = control;
            _control.SubmitClicked += SubmitClickedAsync;
            _service.ChatMessageCompleted += ChatMessageCompleted;
            _service.ChatMessageStream += ChatMessageStream;
            if (_bot == null)
                _bot = await _storageService.GetChatbotAsync(1, cancellationToken).ConfigureAwait(false);
        }

        private async void SubmitClickedAsync(object? sender, ChatMessageEventArgs e)
        {
            IsLoading = true;
            IsStreaming = false;
            var settings = new OpenAIPromptExecutionSettings()
            {
                Temperature = 0.7,
            };
            if (_lastMessage == null)
                _lastMessage = await _service.SubmitMessageToNewSession(_bot.Id, e.UserMessage, settings: settings);
            else
                _lastMessage = await _service.AppendMessageToSession(e.UserMessage, _lastMessage.ChatSessionId, settings: settings);

            IsLoading = false;
            IsStreaming = false;

            Messages.Clear();
            Messages.AddRange(_lastMessage.ChatSession.ChatMessages);
            if(Messages.Count == 1)
            {
                await UpdateSessionTitleBasedOnHistoryAsync();
            }
            OnUpdateState();
        }

        public async Task UpdateSessionTitleBasedOnHistoryAsync()
        {
            await UpdateSessionTitleAsync(await GetChatTitleAsync());
        }

        public async Task UpdateSessionTitleAsync(string title)
        {
            _lastMessage.ChatSession.Title = title;
            await _storageService.UpdateChatSessionAsync(_lastMessage.ChatSession);
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
            foreach (var msg in _lastMessage.ChatSession.ChatMessages)
            {
                body.AppendFormat("User Message: {0}\n\nAgent Response: {1}\n", msg.UserMessage,msg.AgentResponse);
            }
            var settings = new OpenAIPromptExecutionSettings()
            {
                Temperature = 0.8,
                ModelId = "gpt-3.5-turbo"
            };
            var res = await _service.GetChatMessageAsync(prompt.Replace("{body}", body.ToString()), settings);
            var content = res.Items.Where(i => i is TextContent).Select(i => i as TextContent).ToList();
            return string.Join(Environment.NewLine, content);
        }
        private void ChatMessageCompleted(object? sender, ChatMessageCompletedEventArgs e)
        {
        }

        private void ChatMessageStream(object? sender, ChatMessageStreamEventArgs e)
        {
            IsLoading = false;
            IsStreaming = true;
            OnUpdateState();
        }

        protected virtual void OnUpdateState()
        {
            UpdateState?.Invoke(this, EventArgs.Empty);
        }
    }
}
