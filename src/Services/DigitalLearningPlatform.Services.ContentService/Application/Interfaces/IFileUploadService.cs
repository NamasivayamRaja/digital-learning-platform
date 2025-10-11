using DigitalLearningPlatform.Services.ContentService.Application.DTOs;

namespace DigitalLearningPlatform.Services.ContentService.Application.Interfaces
{
    public interface IFileUploadService
    {
        Task<InitiateFileUploadResponseDto> InitiateUploadAsync(InitiateFileUploadRequestDto request);
        Task<bool> CompleteUploadAsync(Guid uploadId);

    }
}
