using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MyCloset.Services.Interfaces;

namespace MyCloset.Services.Implementation
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        private readonly ILogger<BlobStorageService> _logger;

        public BlobStorageService(ILogger<BlobStorageService> logger, IConfiguration configuration)
        {
            _logger = logger;
            string? connectionString = configuration["AzureBlobStorage"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("AzureBlobStorage configuration is missing");
            }
            _blobServiceClient = new BlobServiceClient(new Uri(connectionString), new DefaultAzureCredential());
            
            _containerName = configuration["BlobContainerName"] ?? "clothing-images";
        }

        public async Task<string> UploadImageAsync(Guid userId, string image)
        {
            string blobName = userId.ToString() + "-" + Guid.NewGuid().ToString();
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            var imageBytes = Convert.FromBase64String(image);

            using (var stream = new MemoryStream(imageBytes))
            {
                await blobClient.UploadAsync(stream, true);
            }

            return blobClient.Uri.ToString();
        }

        public async Task<string> UploadImageAsync(Guid userId, IFormFile image)
        {
            string blobName = userId.ToString() + "-" + Guid.NewGuid().ToString();
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = image.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            return blobClient.Uri.ToString();
        }

        public async Task<byte[]> GetImageAsync(string blobUri)
        {
            BlobClient blobClient = new BlobClient(new Uri(blobUri), new DefaultAzureCredential());
            BlobDownloadInfo blobDownloadInfo = await blobClient.DownloadAsync();

            using (var memoryStream = new MemoryStream())
            {
                await blobDownloadInfo.Content.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }

}
