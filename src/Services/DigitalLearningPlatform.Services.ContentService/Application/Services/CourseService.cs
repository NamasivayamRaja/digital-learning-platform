using DigitalLearningPlatform.BuildingBlocks.Common.Exceptions;
using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Application.Interfaces;
using DigitalLearningPlatform.Services.ContentService.Domain;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces;
using Mapster;

namespace DigitalLearningPlatform.Services.ContentService.Application.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateCourseResponseDto> CreateCourseAsync(CreateCourseRequestDto dto)
        {
            if(await _unitOfWork.Courses.ExistsWithTitleAsync(dto.Title))
            {
                throw new LearningPlatformException("Course title already exist", StatusCodes.Status409Conflict);
            }

            var course = new Course(dto.Title, dto.Description, dto.Overview, dto.AuthorId, dto.Category, dto.Level);

            _unitOfWork.Courses.Add(course);

            if(await _unitOfWork.SaveChangesAsync())
            {
                return course.Adapt<CreateCourseResponseDto>();
            }

            throw new LearningPlatformException("Course failed to save");
        }

        public async Task DeleteCourseAsync(Guid authorId, Guid courseId)
        {
            Course course = await GetCourseAndValidateAsync(authorId, courseId);

            _unitOfWork.Courses.Delete(course);

            if (!await _unitOfWork.SaveChangesAsync())
                throw new LearningPlatformException("Course deletion failed");
        }

        public async Task<CourseDto?> GetCourseByIdAsync(Guid id)
        {
            var result = await _unitOfWork.Courses.GetByIdAsync(id);

            if (result == null)
                return null;

            return result.Adapt<CourseDto>();
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesByAuthor(Guid authorId)
        {
            var result =  await _unitOfWork.Courses.GetCoursesByAuthor(authorId);

            if (result == null)
            {
                return Enumerable.Empty<CourseDto>();
            }           

            return result.Adapt<IEnumerable<CourseDto>>();
        }

        public async Task UpdateCourseAsync(Guid authorId, UpdateCourseRequestDto dto)
        {
            if (await _unitOfWork.Courses.ExistsWithTitleAsync(dto.Title, dto.Id))
            {
                throw new LearningPlatformException("Course title already exist", StatusCodes.Status409Conflict);
            }

            Course course = await GetCourseAndValidateAsync(authorId, dto.Id);

            dto.Adapt(course);
            
            _unitOfWork.Courses.Update(course);

            if (!await _unitOfWork.SaveChangesAsync())
                throw new LearningPlatformException("Course update failed");

        }

        public async Task<Course> GetCourseAndValidateAsync(Guid authorId, Guid courseId)
        {
            // Single database query that checks both existence and authorization
            var course = await _unitOfWork.Courses.GetByIdWithAuthorizationAsync(courseId, authorId);

            if (course == null)
            {
                throw new LearningPlatformException("Course does not exist or you don't have permission to access it", StatusCodes.Status404NotFound);
            }

            return course;
        }
    }
}
