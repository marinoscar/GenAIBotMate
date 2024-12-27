using Luval.GenAIBotMate.Components;
using Luval.GenAIBotMate.Core.Entities;
using Luval.GenAIBotMate.Core.Services;
using Luval.GenAIBotMate.Infrastructure.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Diagnostics;

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
            OnUpdateState();
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
