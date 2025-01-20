using Luval.AuthMate.Core.Interfaces;
using Luval.GenAIBotMate.Core.Entities;
using Luval.GenAIBotMate.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GenAIBotMate.Core.Resolver
{
    /// <summary>
    /// Resolves and manages GenAIBot instances.
    /// </summary>
    public class GenAIBotResolver
    {
        private readonly IGenAIBotStorageService _botStorageService;
        private readonly IConfiguration _config;
        private readonly IUserResolver _userResolver;
        private readonly ILogger<GenAIBotResolver> _logger;
        private record BotInfo(string Name, string AccountEmail);
        private static Dictionary<BotInfo, GenAIBot> _bots = new Dictionary<BotInfo, GenAIBot>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GenAIBotResolver"/> class.
        /// </summary>
        /// <param name="botStorageService">The bot storage service.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="userResolver">The user resolver.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the parameters are null.</exception>
        public GenAIBotResolver(IGenAIBotStorageService botStorageService, IConfiguration config, IUserResolver userResolver, ILogger<GenAIBotResolver> logger)
        {
            _botStorageService = botStorageService ?? throw new ArgumentNullException(nameof(botStorageService));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _userResolver = userResolver ?? throw new ArgumentNullException(nameof(userResolver));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the bot asynchronously.
        /// </summary>
        /// <param name="botName">Name of the bot.</param>
        /// <param name="createIfDoesBotExists">if set to <c>true</c> creates the bot if it does not exist.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The GenAIBot instance.</returns>
        /// <exception cref="ArgumentException">Thrown when botName is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the bot does not exist and createIfDoesBotExists is false.</exception>
        public async Task<GenAIBot> GetBotAsync(string botName, bool createIfDoesBotExists = false, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(botName))
                throw new ArgumentException("Bot name cannot be null or empty.", nameof(botName));

            var botInfo = new BotInfo(botName, _userResolver.GetUserEmail());
            if (_bots.TryGetValue(botInfo, out var bot))
                return bot;

            bot = await _botStorageService.GetChatbotAsync(botName, cancellationToken);
            if (bot != null)
            {
                if (!string.IsNullOrEmpty(GetSystemPrompt(botName)))
                {
                    bot.SystemPrompt = GetSystemPrompt(botName);
                    await _botStorageService.UpdateChatbotAsync(bot, cancellationToken);
                }
                _bots.Add(botInfo, bot);
                return bot;
            }

            if (!createIfDoesBotExists)
                throw new InvalidOperationException($"The bot {botName} does not exist.");

            var newBot = new GenAIBot()
            {
                Name = botName,
                AccountId = _userResolver.GetUser().AccountId,
                SystemPrompt = GetSystemPrompt(botName),
                UpdatedBy = _userResolver.GetUserEmail(),
                CreatedBy = _userResolver.GetUserEmail(),
                UtcCreatedOn = DateTime.UtcNow,
                UtcUpdatedOn = DateTime.UtcNow,
                Version = 1,
            };

            await _botStorageService.CreateChatbotAsync(newBot, cancellationToken);
            _bots.Add(botInfo, newBot);
            return newBot;
        }

        /// <summary>
        /// Gets the system prompt for the specified bot name.
        /// </summary>
        /// <param name="botName">Name of the bot.</param>
        /// <returns>The system prompt.</returns>
        private string GetSystemPrompt(string botName)
        {
            return _config[$"Bot:{botName}:SystemPrompt"] ?? string.Empty;
        }
    }
}
