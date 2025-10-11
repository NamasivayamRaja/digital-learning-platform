using DigitalLearningPlatform.BuildingBlocks.Common.Domain;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;

namespace DigitalLearningPlatform.Services.ContentService.Domain
{
    public class CourseFileUpload : AuditableEntityBase
    {
        public Guid CourseId { get; private set; }
        public Guid? SectionId { get; private set; }
        public Guid? SectionFileId { get; private set; }
        public string FileName { get; private set; } = string.Empty;
        public string ContentType { get; private set; } = string.Empty;
        public long Size { get; private set; }
        public string? BlobUrl { get; private set; }
        public FileProcessingStatus Status { get; private set; }
        public string? Message { get; private set; }

        public CourseFileUpload(Guid courseId, string fileName, string contentType, long size)
        {
            CourseId = courseId;
            FileName = fileName;
            ContentType = contentType;
            Size = size;
            Status = FileProcessingStatus.Pending;
        }

        public void  UpdateStatus(FileProcessingStatus status) 
        {
            Status = status;
        }
    }
}
