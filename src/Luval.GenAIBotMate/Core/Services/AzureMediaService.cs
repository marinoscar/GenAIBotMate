using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Luval.GenAIBotMate.Infrastructure.Configuration;
using Luval.GenAIBotMate.Infrastructure.Data;
using Luval.GenAIBotMate.Infrastructure.Interfaces;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GenAIBotMate.Core.Services
{
    /// <summary>
    /// Service for handling media operations such as uploading files to blob storage.
    /// </summary>
    public class AzureMediaService : IMediaService
    {
        private readonly MediaServiceConfig _config;
        private readonly ILogger<AzureMediaService> _logger;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureMediaService"/> class.
        /// </summary>
        /// <param name="config">The configuration settings for the media service.</param>
        /// <param name="logger">The logger instance for logging information.</param>
        /// <exception cref="ArgumentNullException">Thrown when config or logger is null.</exception>
        public AzureMediaService(MediaServiceConfig config, ILogger<AzureMediaService> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _blobServiceClient = new BlobServiceClient(_config.ConnectionString);
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_config.ContainerName);
        }

        /// <summary>
        /// Uploads a media file to the blob storage.
        /// </summary>
        /// <param name="content">The byte array content of the media file.</param>
        /// <param name="fileName">The name of the file to be uploaded.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the media file information.</returns>
        /// <exception cref="ArgumentNullException">Thrown when content or fileName is null.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during the upload process.</exception>
        public async Task<MediaFileInfo> UploadMediaAsync(byte[] content, string fileName, CancellationToken cancellationToken)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            return await UploadMediaAsync(new MemoryStream(content), fileName, cancellationToken);
        }


        /// <summary>
        /// Uploads a media file to the blob storage.
        /// </summary>
        /// <param name="stream">The stream content of the media file.</param>
        /// <param name="fileName">The name of the file to be uploaded.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the media file information.</returns>
        /// <exception cref="ArgumentNullException">Thrown when stream or fileName is null.</exception>
        /// <exception cref="ArgumentException">Thrown when stream is not readable.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during the upload process.</exception>
        public async Task<MediaFileInfo> UploadMediaAsync(Stream stream, string fileName, CancellationToken cancellationToken)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(nameof(fileName));
            if (!stream.CanRead)
            {
                _logger.LogError("Stream must be readable");
                throw new ArgumentException("Stream must be readable", nameof(stream));
            }
            try
            {
                //get a unique file name for the blob
                var providerFileName = Guid.NewGuid().ToString().Replace("-", "").ToUpper();

                _logger.LogInformation("Starting upload of file {FileName}", fileName);

                //Creates the container if it doesn't exist
                await _blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

                //Get a reference to the blob file using the unique name
                var blobClient = _blobContainerClient.GetBlobClient(providerFileName);
                //Upload the file to the blob storage
                var res = await blobClient.UploadAsync(stream, true, cancellationToken);
                await blobClient.SetMetadataAsync(new Dictionary<string, string>() { { "DeviceFileName", fileName } }, null, cancellationToken);

                //Gets the properties
                var props = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);

                _logger.LogInformation("Successfully uploaded file {FileName}", fileName);

                return new MediaFileInfo
                {
                    FileName = fileName, //local file nmae
                    ProviderFileName = providerFileName, //file name on the cloud
                    Uri = blobClient.Uri, //uri of the file on the cloud
                    ContentMD5 = Convert.ToBase64String(res.Value.ContentHash),
                    ContentType = props.Value.ContentType
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while uploading file {FileName}", fileName);
                throw;
            }
        }

        /// <summary>
        /// Asynchronously retrieves media file information from the provider file name.
        /// </summary>
        /// <param name="providerFileName">The provider file name of the media file.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the media file information.</returns>
        /// <exception cref="ArgumentNullException">Thrown when providerFileName is null or empty.</exception>
        /// <exception cref="Exception">Thrown when an error occurs while retrieving the media file information.</exception>
        public async Task<MediaFileInfo> GetMediaInfoFromProviderFileName(string providerFileName, CancellationToken cancellationToken = default)
        {
            var blobClient = _blobContainerClient.GetBlobClient(providerFileName);
            var props = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
            props.Value.Metadata.TryGetValue("DeviceFileName", out string? fileName);
            return new MediaFileInfo
            {
                ProviderFileName = providerFileName,
                ContentType = props.Value.ContentType,
                ContentMD5 = Convert.ToBase64String(props.Value.ContentHash),
                FileName = fileName,
                ProviderName = "Azure",
                PublicUri = new Uri(GetPublicUrl(providerFileName)),
                Uri = blobClient.Uri
            };
        }

        /// <summary>
        /// Asynchronously gets the public URL for a media file.
        /// </summary>
        /// <param name="providerFileName">The provider file name of the media file.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the public URL of the media file.</returns>
        /// <exception cref="ArgumentNullException">Thrown when providerFileName is null or empty.</exception>
        /// <exception cref="Exception">Thrown when an error occurs while generating the public URL.</exception>
        public Task<string> GetPublicUrlAsync(string providerFileName, CancellationToken cancellationToken)
        {
            return Task.Run(() => GetPublicUrl(providerFileName), cancellationToken);
        }

        private string GetPublicUrl(string providerFileName)
        {
            if (string.IsNullOrEmpty(providerFileName))
            {
                _logger.LogError("Provider file name is null or empty");
                throw new ArgumentNullException(nameof(providerFileName), "Provider file name cannot be null or empty");
            }

            try
            {
                var blobClient = _blobContainerClient.GetBlobClient(providerFileName);
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = _config.ContainerName,
                    BlobName = providerFileName,
                    Resource = "b",
                    StartsOn = DateTimeOffset.UtcNow,
                    ExpiresOn = DateTimeOffset.UtcNow.Add(_config.SASExpiration)
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Read);
                var keyCredential = new Azure.Storage.StorageSharedKeyCredential(_blobContainerClient.AccountName, ExtractAccountKey(_config.ConnectionString));
                var sasToken = sasBuilder.ToSasQueryParameters(keyCredential).ToString();
                var uriBuilder = new UriBuilder(blobClient.Uri)
                {
                    Query = sasToken
                };
                return uriBuilder.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating public URL for file {ProviderFileName}", providerFileName);
                throw;
            }
        }

        internal string ExtractAccountKey(string connectionString)
        {
            // Split the connection string into key-value pairs
            string[] pairs = connectionString.Split(';');
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            foreach (var pair in pairs)
            {
                if (!string.IsNullOrEmpty(pair))
                {
                    var keyValue = pair.Split('=', 2);
                    if (keyValue.Length == 2)
                    {
                        keyValuePairs[keyValue[0]] = keyValue[1];
                    }
                }
            }

            // Retrieve the AccountKey
            if (keyValuePairs.TryGetValue("AccountKey", out string accountKey))
            {
                return accountKey;
            }
            throw new Exception("AccountKey not found.");
        }
    }
}
