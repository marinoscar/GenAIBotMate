using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Luval.GenAIBotMate.Core.Entities
{
    /// <summary>
    /// Represents a chat agent entity with details such as name, description, image, prompts, and display color.
    /// </summary>
    [Table("GenAIBot")]
    public class GenAIBot
    {
        /// <summary>
        /// The unique identifier for the chat agent.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public ulong Id { get; set; }

        /// <summary>
        /// The foreign key referencing the associated Account.
        /// This can be used for multi tenancy scenarios.
        /// Default to 1 for single tenant.
        /// </summary>
        [Required(ErrorMessage = "AccountId is required.")]
        [Column("AccountId")]
        [DefaultValue(1ul)]
        public ulong AccountId { get; set; } = 1ul;

        /// <summary>
        /// The name of the chat agent.
        /// </summary>
        [Required(ErrorMessage = "Name is required.")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        [Column("Name")]
        public string Name { get; set; }

        /// <summary>
        /// A brief description of the chat agent.
        /// </summary>
        [MaxLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
        [Column("Description")]
        public string? Description { get; set; }

        /// <summary>
        /// The URL of an image representing the chat agent.
        /// </summary>
        [MaxLength(500, ErrorMessage = "ImageUrl cannot exceed 500 characters.")]
        [Column("ImageUrl")]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// The system prompt used for initializing conversations with the chat agent.
        /// </summary>
        [Column("SystemPrompt")]
        public string? SystemPrompt { get; set; }

        /// <summary>
        /// The safety prompt used for managing responses from the chat agent.
        /// </summary>
        [Column("SafetyPrompt")]
        public string? SafetyPrompt { get; set; }

        /// <summary>
        /// The color used to represent the chat agent in the system.
        /// </summary>
        [MaxLength(25, ErrorMessage = "SystemColor must be a valid hex color code.")]
        [Column("SystemColor")]
        public string? SystemColor { get; set; }

        /// <summary>
        /// The date and time when the chat agent was created (UTC).
        /// </summary>
        [Column("UtcCreatedOn")]
        public DateTime UtcCreatedOn { get; set; }

        /// <summary>
        /// The user who created the chat agent.
        /// </summary>
        [Column("CreatedBy")]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// The date and time when the chat agent was last updated (UTC).
        /// </summary>
        [Column("UtcUpdatedOn")]
        public DateTime? UtcUpdatedOn { get; set; }

        /// <summary>
        /// The user who last updated the chat agent.
        /// </summary>
        [Column("UpdatedBy")]
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// The version of the chat agent record.
        /// </summary>
        [Column("Version")]
        public uint Version { get; set; }

        /// <summary>
        /// The collection of chat sessions associated with the chat agent.
        /// </summary>
        public ICollection<ChatSession> ChatSessions { get; set; }

        /// <summary>
        /// Constructor to initialize control fields.
        /// </summary>
        public GenAIBot()
        {
            UtcCreatedOn = DateTime.UtcNow;
            Version = 1;
        }

        /// <summary>
        /// Converts the chat agent object to a JSON-formatted string.
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
