namespace DigitalLearningPlatform.Services.ContentService.Application.Interfaces
{
    public interface IStorageService
    {
        Task<string> GenerateUploadSasUriAsync(string containerName, string blobName, TimeSpan validFor, string contentType);
    }

}
