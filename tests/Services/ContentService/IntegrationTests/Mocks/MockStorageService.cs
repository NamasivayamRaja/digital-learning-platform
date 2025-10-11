using DigitalLearningPlatform.Services.ContentService.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace DigitalLearningPlatform.Services.ContentService.IntegrationTests.Mocks
{
    public class MockStorageService : IStorageService
    {
        public Task<string> GenerateUploadSasUriAsync(string containerName, string blobName, TimeSpan validFor, string contentType)
        {
            var fakeSasUrl = $"https://fake.blob.core.windows.net/{containerName}/{blobName}?sv=2022-11-02&ss=bfqt&srt=sco&sp=rwdlacupiytfx&se=2025-08-21T18:09:37Z&st=2025-08-21T10:09:37Z&spr=https&sig=fakeSignature";
            return Task.FromResult(fakeSasUrl);
        }
    }
}
