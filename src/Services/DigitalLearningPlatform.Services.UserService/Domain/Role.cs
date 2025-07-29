using DigitalLearningPlatform.BuildingBlocks.Common.Domain;

namespace DigitalLearningPlatform.Services.UserService.Domain
{
    public class Role : EntityBase
    {
        public required string Name { get; set; }
    }
}
