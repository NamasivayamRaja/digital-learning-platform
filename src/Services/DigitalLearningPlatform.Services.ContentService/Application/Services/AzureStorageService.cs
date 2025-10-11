using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using DigitalLearningPlatform.Services.ContentService.Application.Interfaces;

namespace DigitalLearningPlatform.Services.ContentService.Application.Services
{
    public class AzureStorageService : IStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public AzureStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string> GenerateUploadSasUriAsync(
            string containerName,
            string blobName,
            TimeSpan validFor,
            string contentType)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            await containerClient.CreateIfNotExistsAsync();

            if (!_blobServiceClient.CanGenerateAccountSasUri)
                throw new InvalidOperationException("BlobServiceClient is not authorized to generate SAS.");

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                ExpiresOn = DateTimeOffset.UtcNow.Add(validFor),
                Resource = "b"
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Write | BlobSasPermissions.Create);

            sasBuilder.ContentType = contentType;

            Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

            return sasUri.ToString();
        }

    }
}
