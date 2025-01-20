using Luval.GenAIBotMate.Infrastructure.Data;

namespace Luval.GenAIBotMate.Infrastructure.Interfaces
{
    /// <summary>
    /// Provides methods for media file operations.
    /// </summary>
    public interface IMediaService
    {
        /// <summary>
        /// Asynchronously gets the public URL for a media file.
        /// </summary>
        /// <param name="providerFileName">The provider file name of the media file.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the public URL of the media file.</returns>
        /// <exception cref="ArgumentNullException">Thrown when providerFileName is null or empty.</exception>
        /// <exception cref="Exception">Thrown when an error occurs while generating the public URL.</exception>
        Task<string> GetPublicUrlAsync(string providerFileName, CancellationToken cancellationToken);

        /// <summary>
        /// Uploads a media file to the blob storage.
        /// </summary>
        /// <param name="content">The byte array content of the media file.</param>
        /// <param name="fileName">The name of the file to be uploaded.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the media file information.</returns>
        /// <exception cref="ArgumentNullException">Thrown when content or fileName is null.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during the upload process.</exception>
        Task<MediaFileInfo> UploadMediaAsync(byte[] content, string fileName, CancellationToken cancellationToken);

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
        Task<MediaFileInfo> UploadMediaAsync(Stream stream, string fileName, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously retrieves media file information from the provider file name.
        /// </summary>
        /// <param name="providerFileName">The provider file name of the media file.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the media file information.</returns>
        /// <exception cref="ArgumentNullException">Thrown when providerFileName is null or empty.</exception>
        /// <exception cref="Exception">Thrown when an error occurs while retrieving the media file information.</exception>
        Task<MediaFileInfo> GetMediaInfoFromProviderFileName(string providerFileName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a media file from the blob storage.
        /// </summary>
        /// <param name="providerFileName">The provider file name of the media file to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when providerFileName is null or empty.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during the delete process.</exception>
        Task DeleteMediaAsync(string providerFileName, CancellationToken cancellationToken = default);
    }
}