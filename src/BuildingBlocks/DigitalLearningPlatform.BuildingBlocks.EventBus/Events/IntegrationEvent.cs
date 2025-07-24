using System.Text.Json.Serialization;

namespace DigitalLearningPlatform.BuildingBlocks.EventBus.Events
{
    public class IntegrationEvent : EventBase
    {
        public string Publisher { get; private set; } = string.Empty;
        public int Version { get; set; } = 1;

        [JsonConstructor]
        public IntegrationEvent() : base() { 

        }

        public IntegrationEvent(string publisher) : base()
        {
            Publisher = publisher;
        }

        public IntegrationEvent(string publisher, int version) : this(publisher) { 
            Version = version;
        }
    }
}
