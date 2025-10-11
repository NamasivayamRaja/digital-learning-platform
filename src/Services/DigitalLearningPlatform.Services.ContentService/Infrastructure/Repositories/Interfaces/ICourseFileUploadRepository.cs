using DigitalLearningPlatform.Services.ContentService.Domain;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;

namespace DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces
{
    public interface ICourseFileUploadRepository
    {
        /// <summary>
        /// Gets a CourseFileUpload by its ID.
        /// </summary>
        Task<CourseFileUpload?> GetByIdAsync(Guid id);

        /// <summary>
        /// Adds a new CourseFileUpload to the context.
        /// </summary>
        void Add(CourseFileUpload fileUpload);

        /// <summary>
        /// Updates an existing CourseFileUpload.
        /// </summary>
        void Update(CourseFileUpload fileUpload);

        /// <summary>
        /// Gets uploads for a given course by id
        /// </summary>
        Task<IEnumerable<CourseFileUpload>> GetUploadsByCourseIdAsync(Guid courseId);

        /// <summary>
        /// Gets uploads for a given course by id and status
        /// </summary>
        Task<IEnumerable<CourseFileUpload>> GetUploadsByCourseIdAndStatusAsync(Guid courseId, FileProcessingStatus status);

    }
}
