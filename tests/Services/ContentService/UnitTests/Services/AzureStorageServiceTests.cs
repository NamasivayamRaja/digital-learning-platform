using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using DigitalLearningPlatform.Services.ContentService.Application.Services;
using FluentAssertions;
using Moq;

namespace DigitalLearningPlatform.Services.ContentService.UnitTests.Services
{
    public class AzureStorageServiceTests
    {
        private readonly Mock<BlobServiceClient> _mockBlobServiceClient;
        private readonly Mock<BlobContainerClient> _mockContainerClient;
        private readonly Mock<BlobClient> _mockBlobClient;
        private readonly AzureStorageService _storageService;

        public AzureStorageServiceTests()
        {
            _mockBlobServiceClient = new Mock<BlobServiceClient>();
            _mockContainerClient = new Mock<BlobContainerClient>();
            _mockBlobClient = new Mock<BlobClient>();
            _storageService = new AzureStorageService(_mockBlobServiceClient.Object);
        }

        [Fact]
        public async Task GenerateUploadSasUriAsync_ShouldReturnValidSasUri()
        {
            // Arrange
            var containerName = "test-container";
            var blobName = "test-blob";
            var validFor = TimeSpan.FromMinutes(15);
            var contentType = "video/mp4";
            
            var expectedUri = new Uri("https://fakestorage.blob.core.windows.net/test-container/test-blob.txt?sasToken");


            _mockBlobServiceClient.Setup(x => x.GetBlobContainerClient(containerName)).Returns(_mockContainerClient.Object);

            _mockContainerClient.Setup(x => x.GetBlobClient(blobName)).Returns(_mockBlobClient.Object);

            _mockBlobServiceClient.Setup(x => x.CanGenerateAccountSasUri).Returns(true);

            _mockBlobClient.Setup(x=>x.GenerateSasUri(It.IsAny<BlobSasBuilder>())).Returns(expectedUri);

            var sasUri = await _storageService.GenerateUploadSasUriAsync(containerName, blobName, validFor, contentType);

            sasUri.Should().NotBeNull();
            sasUri.Should().Be(expectedUri.ToString());
        }
    }
}
