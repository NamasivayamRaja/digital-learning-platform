using DigitalLearningPlatform.BuildingBlocks.Common.Domain;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;

namespace DigitalLearningPlatform.Services.ContentService.Domain
{
    public class Course : AuditableEntityBase
    {
        public string Title { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public string Overview { get; private set; } = default!; // Optional long-form
        public Guid AuthorId { get; private set; }
        public AuthorLookUp? Author {  get; private set; }
        public CourseCategory Category { get; private set; }
        public CourseLevel Level { get; private set; }
        public CreationStatus Status { get; private set; }
        public TimeSpan TotalLength { get; private set; } // Sum of section video lengths

        // Navigation
        public List<Section> Sections { get; private set; } = new();

        // EF Core constructor
        private Course() : base() { }

        public Course(string title, string description, string overview, Guid authorId,
                      CourseCategory category, CourseLevel level, CreationStatus status = CreationStatus.Draft)
        {
            Title = title;
            Description = description;
            Overview = overview;
            AuthorId = authorId;
            Category = category;
            Level = level;
            Status = CreationStatus.Approved;// Once the Admin Account profile setup, status flow will be corrected
            TotalLength = TimeSpan.Zero;
        }

        // Behaviors
        public void SetStatus(CreationStatus newstatus) => Status = newstatus;
        public void UpdateTotalLength()
        {
            TotalLength = Sections.Aggregate(TimeSpan.Zero, (sum, s) => sum + (s.VideoLength ?? TimeSpan.Zero));
        }
    }
}
