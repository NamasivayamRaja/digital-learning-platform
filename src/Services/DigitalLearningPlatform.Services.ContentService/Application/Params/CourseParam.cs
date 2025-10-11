using DigitalLearningPlatform.BuildingBlocks.Common.Helper;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;

namespace DigitalLearningPlatform.Services.ContentService.Application.Params
{
    public class CourseParam : PaginationParam
    {
        public string? Title { get; set; }
        public CourseCategory? Category { get; set; }
    }
}
