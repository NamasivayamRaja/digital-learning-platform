using DigitalLearningPlatform.BuildingBlocks.EventBus.Events;

namespace DigitalLearningPlatform.BuildingBlocks.EventBus.Abstractions
{
    // In IEventBus.cs - the core Observer pattern implementation
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);

        void Subscribe<T,TH>() where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler;

        void Unsubscribe<T, TH>() where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler;

    }
}
