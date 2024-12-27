using Luval.GenAIBotMate.Components;
using Luval.GenAIBotMate.Core.Entities;
using Luval.GenAIBotMate.Core.Services;
using Luval.GenAIBotMate.Infrastructure.Interfaces;
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
            _service = service;
            _storageService = storageService;
        }

        public void Initialize(GenAIBotMateControl control)
        {
            _control = control;
            _control.SubmitClicked += SubmitClickedAsync;
            _service.ChatMessageCompleted += ChatMessageCompleted;
            _service.ChatMessageStream += ChatMessageStream;
            if(_bot == null)
                _bot = _storageService.GetChatbotAsync(1).Result;
        }

        private async void SubmitClickedAsync(object? sender, ChatMessageEventArgs e)
        {
            IsLoading = true;
            IsStreaming = false;
            if (_lastMessage == null)
                _lastMessage = await _service.SubmitMessageToNewSession(_bot.Id, e.UserMessage, temperature:0.7);
            else
                _lastMessage = await _service.AppendMessageToSession(e.UserMessage, _lastMessage.ChatSessionId, temperature: 0.7);

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
