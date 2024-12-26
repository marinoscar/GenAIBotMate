using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Luval.GenAIBotMate.Core.Entities
{
    /// <summary>
    /// Represents an individual message exchanged in a chat session.
    /// </summary>
    [Table("ChatMessage")]
    public class ChatMessage
    {
        /// <summary>
        /// The unique identifier for the chat message.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public ulong Id { get; set; }

        /// <summary>
        /// The foreign key referencing the chat session.
        /// </summary>
        [Required(ErrorMessage = "ChatId is required.")]
        [Column("ChatSessionId")]
        public ulong ChatSessionId { get; set; }

        /// <summary>
        /// Navigation property for the associated chat session.
        /// </summary>
        [ForeignKey(nameof(ChatSessionId))]
        public virtual ChatSession ChatSession { get; set; }

        /// <summary>
        /// The user's message in the chat.
        /// </summary>
        [Required(ErrorMessage = "UserMessage is required.")]
        [Column("UserMessage")]
        public string UserMessage { get; set; }

        /// <summary>
        /// The AI agent's response in the chat.
        /// </summary>
        [Required(ErrorMessage = "AgentResponse is required.")]
        [Column("AgentResponse")]
        public string AgentResponse { get; set; }

        /// <summary>
        /// The URL of the media attachment in the AI agent's response.
        /// </summary>
        [Column("AgentResponseMediaUrl")]
        public string? AgentResponseMediaUrl { get; set; }

        /// <summary>
        /// The name of the AI model used for the response.
        /// </summary>
        [Column("Model")]
        public string? Model { get; set; }

        /// <summary>
        /// The name of the AI provider used for the response.
        /// </summary>
        [Column("ProviderName")]
        public string? ProviderName { get; set; }

        /// <summary>
        /// The number of tokens in the user's message.
        /// </summary>
        [Required(ErrorMessage = "InputTokens is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "InputTokens must be a positive number.")]
        [Column("InputTokens")]
        public int InputTokens { get; set; }

        /// <summary>
        /// The number of tokens in the AI agent's response.
        /// </summary>
        [Required(ErrorMessage = "OutputTokens is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "OutputTokens must be a positive number.")]
        [Column("OutputTokens")]
        public int OutputTokens { get; set; }

        /// <summary>
        /// The date and time when the message was created (UTC).
        /// </summary>
        [Column("UtcCreatedOn")]
        public DateTime UtcCreatedOn { get; set; }

        /// <summary>
        /// The user who created the message.
        /// </summary>
        [Column("CreatedBy")]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// The date and time when the message was last updated (UTC).
        /// </summary>
        [Column("UtcUpdatedOn")]
        public DateTime? UtcUpdatedOn { get; set; }

        /// <summary>
        /// The user who last updated the message.
        /// </summary>
        [Column("UpdatedBy")]
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// The version of the chat message record.
        /// </summary>
        [Column("Version")]
        public uint Version { get; set; }

        /// <summary>
        /// The media attachments associated with the chat message.
        /// </summary>
        public List<ChatMessageMedia> Media { get; set; } = new List<ChatMessageMedia>();


        /// <summary>
        /// Constructor to initialize control fields.
        /// </summary>
        public ChatMessage()
        {
            UtcCreatedOn = DateTime.UtcNow;
            Version = 1;
        }

        /// <summary>
        /// Converts the chat message object to a JSON-formatted string.
        /// </summary>
        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });
        }
    }

}
