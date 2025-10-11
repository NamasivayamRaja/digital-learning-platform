using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Domain;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLearningPlatform.Services.ContentService.Application.Interfaces
{
    public interface ICourseService
    {
        Task<CreateCourseResponseDto> CreateCourseAsync(CreateCourseRequestDto courseDto);
        Task DeleteCourseAsync(Guid authorId, Guid courseId);
        Task<CourseDto?> GetCourseByIdAsync(Guid id);
        Task<IEnumerable<CourseDto>> GetCoursesByAuthor(Guid authorId);
        Task UpdateCourseAsync(Guid AuthorId, UpdateCourseRequestDto updateCourseDto);
    }
}
