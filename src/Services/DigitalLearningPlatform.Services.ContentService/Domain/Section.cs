using DigitalLearningPlatform.BuildingBlocks.Common.Domain;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;
namespace DigitalLearningPlatform.Services.ContentService.Domain
{
    public class Section : AuditableEntityBase
    {
        public Guid CourseId { get; private set; }
        public string Title { get; private set; } = default!;
        public int Order { get; private set; }
        public CreationStatus Status { get; private set; }

        public List<SectionFile> Files { get; private set; } = new();
        public TimeSpan? VideoLength { get; private set; }

        // EF Core constructor
        private Section() : base() { }

        public Section(Guid courseId, string title, int order, CreationStatus status = CreationStatus.Draft)
        {
            CourseId = courseId;
            Title = title;
            Order = order;
            Status = status;
        }

        public void SetStatus(CreationStatus newStatus)
        {
            Status = newStatus;
        }

        public void SetVideoLength(TimeSpan length)
        {
            VideoLength = length;
        }

        public void UpdateSection(string title, int order, CreationStatus status)
        {
            Title = title;
            Order = order;
            SetStatus(status);
        }
        public Course Course { get; set; } = default!;
    }
}
