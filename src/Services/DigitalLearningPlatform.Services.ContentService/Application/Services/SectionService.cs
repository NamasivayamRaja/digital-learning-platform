using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Application.Interfaces;
using DigitalLearningPlatform.Services.ContentService.Domain;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;
using Mapster;
using DigitalLearningPlatform.BuildingBlocks.Common.Exceptions;
namespace DigitalLearningPlatform.Services.ContentService.Application.Services
{
    public class SectionService : ISectionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SectionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task UpdateSectionAsync(Guid authorId, UpdateSectionRequestDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                Section section = await GetSectionWithAuthorizationAsync(authorId, dto.SectionId);

                section.UpdateSection(dto.Title, dto.Order, dto.Status);

                _unitOfWork.Sections.Update(section);

                if (dto.CreateSectionFiles != null)
                {
                    //only changed records only processed here, there is no chance new section of file logic,
                    //file upload itself it create a new section file and it's link with draft data
                    //delete logic also handled separately
                    //below approach is the standard approach updating the child record, it will not lead to concurrency problem
                    foreach (var sectionFile in dto.CreateSectionFiles)
                    {
                        var updateSection = await _unitOfWork.SectionFiles.GetByIdAsync(sectionFile.SectionFileId);

                        if (updateSection == null)
                            continue;

                        updateSection.UpdateSectionFile(sectionFile.Title, sectionFile.FileName, sectionFile.BlobPath, sectionFile.ContentType, sectionFile.Size, CreationStatus.Completed);

                        _unitOfWork.SectionFiles.Update(updateSection);

                        var courseFile = await _unitOfWork.CourseFileUploads.GetByIdAsync(sectionFile.CourseFileUploadId);

                        if(courseFile == null)
                            continue;

                        courseFile.UpdateStatus(FileProcessingStatus.Associated);

                        _unitOfWork.CourseFileUploads.Update(courseFile);
                    }
                }

                await _unitOfWork.CommitTransactionAsync();

            }
            catch 
            {
                // exception is not handled here because it's not expected exception so it's should be handle by common error handler
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        private async Task<Section> GetSectionWithAuthorizationAsync(Guid authorId, Guid sectionId)
        {
            // Single database query that checks both existence and authorization
            var section = await _unitOfWork.Sections.GetByIdWithAuthorizationAsync(sectionId, authorId);
            
            if (section == null)
            {
                throw new LearningPlatformException("Resource not found", StatusCodes.Status404NotFound);
            }

            return section;
        }

        public async Task DeleteSectionAsync(Guid authorId, Guid sectionId)
        {
            Section section = await GetSectionWithAuthorizationAsync(authorId, sectionId);

            _unitOfWork.Sections.Delete(section);

            if (!await _unitOfWork.SaveChangesAsync())
                throw new LearningPlatformException("Section deletion failed");           
        }

        public async Task<IEnumerable<SectionDto>> GetSectionByCourseIdAndAuthorAsync(Guid authorId, Guid courseId)
        {
            var sections = await _unitOfWork.Sections.GetSectionsByCourseIdAndAuthorAsync(courseId, authorId);

            if (sections == null || !sections.Any())
            {
                return Enumerable.Empty<SectionDto>();
            }
            return await _unitOfWork.Sections.GetSectionsByCourseIdAndAuthorAsync(courseId, authorId);
        }

        public async Task<CreateSectionResponseDto> CreateDraftSectionAsync(Guid authorId, Guid courseId)
        {
            // Validate author has access to the course - this method already includes authorization
            int courseSectionCount = await _unitOfWork.Sections.GetSectionCountByCourseIdAsync(courseId, authorId);
            
            string title = $"Draft - {courseSectionCount}";
            var draftSection = new Section(courseId, title, courseSectionCount, CreationStatus.Draft);

            _unitOfWork.Sections.Add(draftSection);

            if (await _unitOfWork.SaveChangesAsync()) {
                return draftSection.Adapt<CreateSectionResponseDto>();
            }

            throw new LearningPlatformException("Draft creation failed", StatusCodes.Status500InternalServerError);
        }

        public async Task<IEnumerable<SectionFileDto>> GetSectionDetailAsync(Guid sectionId, Guid authorId)
        {
            var sectionFiles = await _unitOfWork.Sections.GetSectionDetailAsync(sectionId, authorId);

            if(sectionFiles == null || !sectionFiles.Any())
            {
                return Enumerable.Empty<SectionFileDto>();
            }

            return sectionFiles;
        }

        public async Task<Guid> CreateSectionFileAsync(InitiateFileUploadRequestDto dto, Guid authorId)
        {
            var section = await GetSectionWithAuthorizationAsync(authorId, dto.SectionId);

            string title = dto.Title ?? dto.FileName;

            var sectionFile = new SectionFile(section.Id, title, dto.FileName, dto.ContentType, dto.FileSize);

            _unitOfWork.SectionFiles.Add(sectionFile);

            if (await _unitOfWork.SaveChangesAsync())
                return sectionFile.Id;

            throw new LearningPlatformException("Section failed to create");
        }
    }
}
