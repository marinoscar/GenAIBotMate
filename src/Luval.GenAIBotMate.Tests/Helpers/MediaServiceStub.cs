using Luval.GenAIBotMate.Infrastructure.Data;
using Luval.GenAIBotMate.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GenAIBotMate.Tests.Helpers
{
    public class MediaServiceStub : IMediaService
    {
        public Task<MediaFileInfo> GetMediaInfoFromProviderFileName(string providerFileName, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPublicUrlAsync(string providerFileName, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return "https://marin2storage.blob.core.windows.net/chatbot/notes.png?sv=2023-01-03&st=2024-12-26T02%3A27%3A06Z&se=2050-12-27T02%3A27%3A00Z&sr=b&sp=r&sig=Z8DB8FNEgICSM7RW9oFFfAQKwIvjZghNt5T643g8xZI%3D"; });
        }

        public Task<MediaFileInfo> UploadMediaAsync(byte[] content, string fileName, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return GetFile(); });
        }

        public Task<MediaFileInfo> UploadMediaAsync(Stream stream, string fileName, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return GetFile(); });
        }

        private MediaFileInfo GetFile()
        {
            return new MediaFileInfo()
            {
                ContentMD5 = "MD5",
                ContentType = "image/png",
                FileName = "notes.png",
                ProviderFileName = "notes.png",
                ProviderName = "Azure",
                Uri = new Uri("https://marin2storage.blob.core.windows.net/chatbot/notes.png"),
                PublicUri = new Uri("https://marin2storage.blob.core.windows.net/chatbot/notes.png?sv=2023-01-03&st=2024-12-26T02%3A27%3A06Z&se=2050-12-27T02%3A27%3A00Z&sr=b&sp=r&sig=Z8DB8FNEgICSM7RW9oFFfAQKwIvjZghNt5T643g8xZI%3D")
            };
        }
    }
}
