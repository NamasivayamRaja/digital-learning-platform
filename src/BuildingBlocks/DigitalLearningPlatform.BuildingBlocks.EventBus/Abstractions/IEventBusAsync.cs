using DigitalLearningPlatform.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningPlatform.BuildingBlocks.EventBus.Abstractions
{
    public interface IEventBusAsync
    {
        Task PublishAsync(IntegrationEvent @event);

        Task SubscribeAsync<T, TH>() where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        Task SubscribeDynamicAsync<TH>(string eventName) where TH : IDynamicIntegrationEventHandler;

        void Unsubscribe<T, TH>() where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler;

    }
}
