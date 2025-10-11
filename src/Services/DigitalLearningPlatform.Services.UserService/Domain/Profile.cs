using DigitalLearningPlatform.BuildingBlocks.Common.Domain;

namespace DigitalLearningPlatform.Services.UserService.Domain
{
    public class Profile : ValueObject
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string Avatar { get; set; } = string.Empty;
        public string? Overview { get; set; }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
        }
    }
}
