using DigitalLearningPlatform.BuildingBlocks.Common.Domain;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;
namespace DigitalLearningPlatform.Services.ContentService.Domain
{
    public class SectionFile : EntityBase
    {
        public Guid SectionId { get; private set; }
        public string Title { get; private set; } = default!;
        public string FileName { get; private set; } = default!;
        public string? BlobPath { get; private set; }
        public string ContentType { get; private set; } = default!; // "video/mp4", "image/png", etc.
        public long Size { get; private set; } // in bytes
        public int Order { get; private set; }
        public CreationStatus Status { get; private set; }

        // EF Core constructor
        private SectionFile() : base() { }

        public SectionFile(Guid sectionId,string title, string fileName, string contentType, long size, CreationStatus status =  CreationStatus.Draft)
        {
            Title = title;
            SectionId = sectionId;
            FileName = fileName;
            ContentType = contentType;
            Size = size;
            SetStatus(status);
        }

        public void SetStatus(CreationStatus newStatus) => Status = newStatus;

        public void UpdateSectionFile(string title, string fileName, string blobPath, string contentType, long size, CreationStatus status)
        {
            Title = title;
            FileName = fileName;
            BlobPath = blobPath;
            ContentType = contentType;
            Size = size;
            SetStatus(status);
        }

        public Section Section { get; set; } = default!; // Navigation property to the Section entity
    }
}
