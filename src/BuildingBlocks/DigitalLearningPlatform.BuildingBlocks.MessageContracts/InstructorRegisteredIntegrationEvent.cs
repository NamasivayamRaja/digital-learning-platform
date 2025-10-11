using DigitalLearningPlatform.BuildingBlocks.EventBus.Events;
using System.Text.Json.Serialization;

namespace DigitalLearningPlatform.BuildingBlocks.MessageContracts
{
    /// <summary>
    /// Integration event published when a user registers as an instructor
    /// Following enterprise patterns: versioning, immutability, and rich metadata
    /// </summary>
    public class InstructorRegisteredIntegrationEvent : IntegrationEvent
    {
        public Guid UserId { get; private set; }
        public string FirstName { get; private set; } = string.Empty; 
        public string LastName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string? Overview { get; private set; }
        public DateTime RegistrationDate { get; private set; }

        /// <summary>
        /// Full name computed property for convenience
        /// </summary>
        public string FullName => $"{FirstName} {LastName}".Trim();

        public InstructorRegisteredIntegrationEvent() : base()
        {
        }

        [JsonConstructor]
        public InstructorRegisteredIntegrationEvent(
            Guid userId, 
            string firstName, 
            string lastName, 
            string email, 
            string? overview,
            DateTime registrationDate) 
            : base("UserService", 1) // Publisher and version
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Overview = overview;
            RegistrationDate = registrationDate;
        }

        public override string ToString()
        {
            return $"InstructorRegisteredIntegrationEvent: UserId={UserId}, Name={FullName}, Email={Email}";
        }
    }
}