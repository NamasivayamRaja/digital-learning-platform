using DigitalLearningPlatform.BuildingBlocks.EventBus.Abstractions;
using DigitalLearningPlatform.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningPlatform.Services.ContentService.IntegrationTests.Mocks
{
    public class MockEventBus : IEventBus
    {
        public List<IntegrationEvent> PublishedEvents { get; } = new List<IntegrationEvent>();

        public Task PublishAsync(IntegrationEvent @event)
        {
            PublishedEvents.Add(@event);
            return Task.CompletedTask;
        }

        public Task SubscribeAsync<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            // Do nothing, not needed for this mock
            return Task.CompletedTask;
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            // Do nothing, not needed for this mock
        }
    }

}
