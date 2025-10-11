using DigitalLearningPlatform.Services.ContentService.Infrastructure.Data;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories
{
    // 
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CourseDbContext _context;
        private readonly ICourseRepository _courseRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly ISectionFileRepository _sectionFileRepository;
        private readonly ICourseFileUploadRepository _courseFileUploadRepository;
        private readonly IAuthorLookUpRepository _authorLookUpRepository;
        private IDbContextTransaction? _currentDbTransaction;

        public UnitOfWork(CourseDbContext context, 
            ICourseRepository courseRepository,
            ISectionRepository sectionRepository,
            ISectionFileRepository sectionFileRepository, 
            ICourseFileUploadRepository courseFileUploadRepository,
            IAuthorLookUpRepository authorLookUpRepository)
        {
            _context = context;
            _courseRepository = courseRepository;
            _sectionRepository = sectionRepository;
            _sectionFileRepository = sectionFileRepository;
            _courseFileUploadRepository = courseFileUploadRepository;
            _authorLookUpRepository = authorLookUpRepository;
        }


        public ICourseRepository Courses => _courseRepository;

        public ISectionRepository Sections => _sectionRepository;

        public ISectionFileRepository SectionFiles => _sectionFileRepository;

        public ICourseFileUploadRepository CourseFileUploads => _courseFileUploadRepository;

        public IAuthorLookUpRepository AuthorLookUps => _authorLookUpRepository;

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentDbTransaction != null)
                return;

            _currentDbTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentDbTransaction == null)
                throw new InvalidOperationException("No transaction started");

            await _context.SaveChangesAsync(cancellationToken);
            await _currentDbTransaction.CommitAsync(cancellationToken);

            await _currentDbTransaction.DisposeAsync();
            _currentDbTransaction = null;
        }

        public void Dispose()
        {
            _currentDbTransaction?.Dispose();
            _context.Dispose();
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentDbTransaction == null)
                throw new InvalidOperationException("No transaction started");

            await _currentDbTransaction.RollbackAsync(cancellationToken);

            await _currentDbTransaction.DisposeAsync();
            _currentDbTransaction = null;
        }

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
