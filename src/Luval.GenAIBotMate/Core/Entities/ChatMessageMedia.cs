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
    /// Represents media associated with a ChatMessage, such as files or attachments.
    /// </summary>
    [Table("ChatMessageMedia")]
    public class ChatMessageMedia
    {
        /// <summary>
        /// The unique identifier for the media record.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public ulong Id { get; set; }

        /// <summary>
        /// The foreign key referencing the associated ChatMessage.
        /// </summary>
        [Required(ErrorMessage = "ChatMessageId is required.")]
        [Column("ChatMessageId")]
        public ulong ChatMessageId { get; set; }

        /// <summary>
        /// Navigation property for the associated ChatMessage.
        /// </summary>
        [ForeignKey(nameof(ChatMessageId))]
        public virtual ChatMessage ChatMessage { get; set; }

        /// <summary>
        /// The URL of the media file.
        /// </summary>
        [Required(ErrorMessage = "MediaUrl is required.")]
        [MaxLength(500, ErrorMessage = "MediaUrl cannot exceed 500 characters.")]
        [Column("MediaUrl")]
        public string MediaUrl { get; set; }

        /// <summary>
        /// The name of the media file on the cloud storage.
        /// </summary>
        [MaxLength(500, ErrorMessage = "ProviderFileName cannot exceed 250 characters.")]
        [Column("ProviderFileName")]
        public string? ProviderFileName { get; set; }

        /// <summary>
        /// The name of the media file on the user device.
        /// </summary>
        [MaxLength(500, ErrorMessage = "Name cannot exceed 250 characters.")]
        [Column("FileName")]
        public string? FileName { get; set; }

        /// <summary>
        /// The content type of the media (e.g., image/jpeg, application/pdf).
        /// </summary>
        [MaxLength(50, ErrorMessage = "ContentType cannot exceed 100 characters.")]
        [Column("ContentType")]
        public string? ContentType { get; set; }

        /// <summary>
        /// The MD5 hash of the media content for integrity verification.
        /// </summary>
        [MaxLength(32, ErrorMessage = "ContentMD5 cannot exceed 32 characters.")]
        [Column("ContentMD5")]
        public string? ContentMD5 { get; set; }

        /// <summary>
        /// The provider name where the media is hosted (e.g., AWS, Azure, GCP).
        /// </summary>
        [Required(ErrorMessage = "ProviderName is required.")]
        [MaxLength(50, ErrorMessage = "ProviderName cannot exceed 50 characters.")]
        [Column("ProviderName")]
        public string ProviderName { get; set; }

        /// <summary>
        /// The date and time when the media record was created (UTC).
        /// </summary>
        [Column("UtcCreatedOn")]
        public DateTime UtcCreatedOn { get; set; }

        /// <summary>
        /// The user who created the media record.
        /// </summary>
        [Column("CreatedBy")]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// The date and time when the media record was last updated (UTC).
        /// </summary>
        [Column("UtcUpdatedOn")]
        public DateTime? UtcUpdatedOn { get; set; }

        /// <summary>
        /// The user who last updated the media record.
        /// </summary>
        [Column("UpdatedBy")]
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// The version of the media record.
        /// </summary>
        [Column("Version")]
        public uint Version { get; set; }

        /// <summary>
        /// Constructor to initialize control fields.
        /// </summary>
        public ChatMessageMedia()
        {
            UtcCreatedOn = DateTime.UtcNow;
            Version = 1;
        }

        /// <summary>
        /// Converts the ChatMessageMedia object to a JSON-formatted string.
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
