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
    /// Represents a chat session, which serves as a container for chat messages.
    /// </summary>
    [Table("ChatSession")]
    public class ChatSession
    {
        /// <summary>
        /// The unique identifier for the chat session.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public ulong Id { get; set; }

        /// <summary>
        /// The foreign key referencing the associated Chatbot.
        /// </summary>
        [Column("GenAIBotId")]
        public ulong GenAIBotId { get; set; }

        /// <summary>
        /// Navigation property for the associated Chatbot.
        /// </summary>
        [ForeignKey(nameof(GenAIBotId))]
        public GenAIBot GenAIBot { get; set; }

        /// <summary>
        /// A reference value to the chat session.
        /// This can be used to reference conversations across different systems.
        /// </summary>
        [Column("ChatReference")]
        public string? ChatReference { get; set; }

        /// <summary>
        /// The title of the chat session.
        /// </summary>
        [Required(ErrorMessage = "Title is required.")]
        [MinLength(3, ErrorMessage = "Title must be at least 3 characters long.")]
        [MaxLength(150, ErrorMessage = "Title cannot exceed 100 characters.")]
        [Column("Title")]
        public string Title { get; set; }


        /// <summary>
        /// Indicates if the chat session can be shared with other users.
        /// </summary>
        [Column("CanShare")]
        public bool CanShare { get; set; }

        /// <summary>
        /// Indicates if the chat session has media attachments.
        /// </summary>
        [Column("HasMedia")]
        public bool HasMedia { get; set; }

        /// <summary>
        /// Indicates if the chat session has been archived.
        /// </summary>
        [Column("IsArchived")]
        public bool IsArchived { get; set; }

        /// <summary>
        /// The date and time when the chat session was created (UTC).
        /// </summary>
        [Column("UtcCreatedOn")]
        public DateTime UtcCreatedOn { get; set; }

        /// <summary>
        /// The user who created the chat session.
        /// </summary>
        [Column("CreatedBy")]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// The date and time when the chat session was last updated (UTC).
        /// </summary>
        [Column("UtcUpdatedOn")]
        public DateTime? UtcUpdatedOn { get; set; }

        /// <summary>
        /// The user who last updated the chat session.
        /// </summary>
        [Column("UpdatedBy")]
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// The version of the chat record.
        /// </summary>
        [Column("Version")]
        public uint Version { get; set; }

        /// <summary>
        /// Navigation property for the associated chat messages.
        /// </summary>
        public ICollection<ChatMessage> ChatMessages { get; set; }

        /// <summary>
        /// Constructor to initialize control fields.
        /// </summary>
        public ChatSession()
        {
            UtcCreatedOn = DateTime.UtcNow;
            Version = 1;
        }

        /// <summary>
        /// Converts the chat session object to a JSON-formatted string.
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
