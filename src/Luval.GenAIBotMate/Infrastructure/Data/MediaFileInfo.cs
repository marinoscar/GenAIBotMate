using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GenAIBotMate.Infrastructure.Data
{
    /// <summary>
    /// Represents information about a media file.
    /// </summary>
    public class MediaFileInfo
    {
        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        public string ProviderName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the URI of the media file.
        /// </summary>
        public Uri Uri { get; set; } = default!;

        /// <summary>
        /// Gets or sets the public URI of the media file.
        /// </summary>
        public Uri PublicUri { get; set; } = default!;

        /// <summary>
        /// Gets or sets the MD5 hash of the content.
        /// </summary>
        public string? ContentMD5 { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the media file.
        /// </summary>
        public string? FileName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the provider's file.
        /// </summary>
        public string ProviderFileName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the content type of the media file.
        /// </summary>
        public string? ContentType { get; set; } = default!;
    }
}
