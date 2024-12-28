using Luval.GenAIBotMate.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GenAIBotMate.Infrastructure.Interfaces
{
    /// <summary>
    /// Interface for managing chatbot storage operations.
    /// </summary>
    public interface IGenAIBotStorageService
    {
        /// <summary>
        /// Creates a new chatbot and saves it to the database.
        /// </summary>
        /// <param name="chatbot">The chatbot entity to create.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The created chatbot entity.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the chatbot is null.</exception>
        Task<GenAIBot> CreateChatbotAsync(GenAIBot chatbot, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a Gen AI Bot by its unique identifier.
        /// </summary>
        /// <param name="botId">The unique identifier of the Gen AI Bot.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The chatbot entity if found; otherwise, null.</returns>
        Task<GenAIBot?> GetChatbotAsync(ulong botId, CancellationToken cancellationToken = default)

        /// <summary>
        /// Retrieves a Gen AI Bot by its unique identifier.
        /// </summary>
        /// <param name="botName">The unique name of the Gen AI Bot.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The chatbot entity if found; otherwise, null.</returns>
        Task<GenAIBot?> GetChatbotAsync(string botName, CancellationToken cancellationToken = default)

        /// <summary>
        /// Updates an existing chatbot and saves the changes to the database.
        /// </summary>
        /// <param name="chatbot">The chatbot entity to update.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated chatbot entity.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the chatbot is null.</exception>
        Task<GenAIBot> UpdateChatbotAsync(GenAIBot chatbot, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a chatbot by its unique identifier.
        /// </summary>
        /// <param name="chatbotId">The unique identifier of the chatbot.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task DeleteChatbotAsync(ulong chatbotId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new chat session and saves it to the database.
        /// </summary>
        /// <param name="chatSession">The chat session entity to create.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The created chat session entity.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the chat session is null.</exception>
        Task<ChatSession> CreateChatSessionAsync(ChatSession chatSession, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing chat session and saves the changes to the database.
        /// </summary>
        /// <param name="chatSession">The chat session entity to update.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated chat session entity.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the chat session is null.</exception>
        Task<ChatSession> UpdateChatSessionAsync(ChatSession chatSession, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a chat session by its unique identifier.
        /// </summary>
        /// <param name="chatSessionId">The unique identifier of the chat session.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task DeleteChatSessionAsync(ulong chatSessionId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a chat session by its unique identifier.
        /// </summary>
        /// <param name="chatSessionId">The unique identifier of the chat session.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The chat session entity if found; otherwise, null.</returns>
        Task<ChatSession?> GetChatSessionAsync(ulong chatSessionId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new chat message to a specified chat session and saves it to the database.
        /// </summary>
        /// <param name="chatSessionId">The unique identifier of the chat session to which the message belongs.</param>
        /// <param name="chatMessage">The chat message entity to add.</param>
        /// <param name="media">A collection of media entities associated with the chat message. Default to null</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The created chat message entity.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the chat message is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the chat session ID is invalid.</exception>
        Task<ChatMessage> AddChatMessageAsync(ulong chatSessionId, ChatMessage chatMessage, IEnumerable<ChatMessageMedia>? media = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds media to a chat message and saves it to the database.
        /// </summary>
        /// <param name="chatMessageId">The unique identifier of the chat message.</param>
        /// <param name="media">The media entity to add.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The created chat message media entity.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the media is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the chat message ID is invalid.</exception>
        Task<ChatMessageMedia> AddMessageMediaAsync(ulong chatMessageId, ChatMessageMedia media, CancellationToken cancellationToken = default);
    }
}
