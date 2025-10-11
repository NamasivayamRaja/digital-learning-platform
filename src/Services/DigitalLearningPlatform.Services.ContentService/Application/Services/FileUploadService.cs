using DigitalLearningPlatform.BuildingBlocks.Common.Exceptions;
using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Application.Interfaces;
using DigitalLearningPlatform.Services.ContentService.Domain;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;

namespace DigitalLearningPlatform.Services.ContentService.Application.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;

        public FileUploadService(IUnitOfWork unitOfWork, IStorageService storageService)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
        }

        public async Task<bool> CompleteUploadAsync(Guid uploadId)
        {
            var courseFile = await _unitOfWork.CourseFileUploads.GetByIdAsync(uploadId);

            if (courseFile == null)
            {
                throw new LearningPlatformException("File is not exist", StatusCodes.Status404NotFound);
            }
            courseFile.UpdateStatus(FileProcessingStatus.Uploaded);

            _unitOfWork.CourseFileUploads.Update(courseFile);

            if(await _unitOfWork.SaveChangesAsync())
            {
                // create a message to publish in rabbitmQ

                return true;
            }

            throw new LearningPlatformException("File upload completion failed");
        }

        public async Task<InitiateFileUploadResponseDto> InitiateUploadAsync(InitiateFileUploadRequestDto dto)
        {
            var courseFileUpload = new CourseFileUpload(dto.CourseId, dto.FileName, dto.ContentType, dto.FileSize);

            _unitOfWork.CourseFileUploads.Add(courseFileUpload);

            if(await _unitOfWork.SaveChangesAsync())
            {
                string blobName = $"{dto.CourseId}/{courseFileUpload.Id}/{dto.FileName}";
                string uploadUrl = await _storageService.GenerateUploadSasUriAsync("CourseUpload", blobName, TimeSpan.FromMinutes(15), dto.ContentType);
                return new InitiateFileUploadResponseDto() { UploadId =  courseFileUpload.Id, UploadUrl = uploadUrl };
            }

            throw new LearningPlatformException("File upload failed.");
        }
    }
}
