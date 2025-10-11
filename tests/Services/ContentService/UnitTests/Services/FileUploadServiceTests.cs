using DigitalLearningPlatform.BuildingBlocks.Common.Exceptions;
using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Application.Interfaces;
using DigitalLearningPlatform.Services.ContentService.Application.Services;
using DigitalLearningPlatform.Services.ContentService.Domain;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DigitalLearningPlatform.Services.ContentService.UnitTests.Services
{
    public class FileUploadServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IStorageService> _mockStorageService;
        private readonly FileUploadService _fileUploadService;

        public FileUploadServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockStorageService = new Mock<IStorageService>();
            _fileUploadService = new FileUploadService(_mockUnitOfWork.Object, _mockStorageService.Object);
        }

        [Fact]
        public async Task InitiateUploadAsync_WithValidDto_ReturnsUploadResponse()
        {
            // Arrange
            var dto = new InitiateFileUploadRequestDto { CourseId = Guid.NewGuid(), SectionId = Guid.NewGuid(), FileName = "test.mp4", ContentType = "video/mp4", FileSize = 12345 };
            var uploadUrl = "http://azure.com/upload";
            var courseFileUpload = new CourseFileUpload(dto.CourseId, dto.FileName, dto.ContentType, dto.FileSize);
            _mockUnitOfWork.Setup(u => u.CourseFileUploads.Add(courseFileUpload));
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockStorageService.Setup(s => s.GenerateUploadSasUriAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<string>()))
                .ReturnsAsync(uploadUrl);

            // Act
            var result = await _fileUploadService.InitiateUploadAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.UploadUrl.Should().Be(uploadUrl);
            _mockUnitOfWork.Verify(u => u.CourseFileUploads.Add(It.IsAny<CourseFileUpload>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CompleteUploadAsync_WhenFileExists_CompletesUpload()
        {
            // Arrange
            var uploadId = Guid.NewGuid();
            var courseFile = new CourseFileUpload(Guid.NewGuid(), "test.mp4", "video/mp4", 12345);
            _mockUnitOfWork.Setup(u => u.CourseFileUploads.GetByIdAsync(uploadId)).ReturnsAsync(courseFile);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _fileUploadService.CompleteUploadAsync(uploadId);

            // Assert
            result.Should().BeTrue();
            courseFile.Status.Should().Be(FileProcessingStatus.Uploaded);
            _mockUnitOfWork.Verify(u => u.CourseFileUploads.Update(courseFile), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CompleteUploadAsync_WhenFileDoesNotExist_ThrowsException()
        {
            // Arrange
            var uploadId = Guid.NewGuid();
            _mockUnitOfWork.Setup(u => u.CourseFileUploads.GetByIdAsync(uploadId)).ReturnsAsync((CourseFileUpload)null!);

            // Act & Assert
            await Assert.ThrowsAsync<LearningPlatformException>(() => _fileUploadService.CompleteUploadAsync(uploadId));
        }
    }
}
