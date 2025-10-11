namespace DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICourseRepository Courses { get; }
        ISectionRepository Sections { get; }
        ISectionFileRepository SectionFiles { get; }
        ICourseFileUploadRepository CourseFileUploads { get; }
        IAuthorLookUpRepository AuthorLookUps { get; }

        /// <summary>
        /// Persists all changes as a single transaction.
        /// </summary>
        Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Begins a new transaction if needed (optional, if manually handling transactions).
        /// </summary>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits the current transaction (optional).
        /// </summary>
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolls back the transaction (optional).
        /// </summary>
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
