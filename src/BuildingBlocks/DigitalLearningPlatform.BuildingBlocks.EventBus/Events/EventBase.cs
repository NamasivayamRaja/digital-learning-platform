using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningPlatform.BuildingBlocks.EventBus.Events
{
    public abstract class EventBase
    {
        public Guid Id { get; private set; }
        public DateTime CreationDate { get; private set; }

        public string EventType => GetType().Name;

        protected EventBase() { 
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }
    }
}
