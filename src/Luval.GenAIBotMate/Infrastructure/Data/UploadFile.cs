using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GenAIBotMate.Infrastructure.Data
{
    /// <summary>
    /// Represents a file to be uploaded.
    /// </summary>
    public class UploadFile
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public string Name { get; set; } = default!;
        /// <summary>
        /// Gets or sets the content of the file.
        /// </summary>
        public Stream Content { get; set; } = default!;
    }
}
