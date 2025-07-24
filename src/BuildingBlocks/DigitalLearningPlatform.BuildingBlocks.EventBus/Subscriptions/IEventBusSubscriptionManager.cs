using DigitalLearningPlatform.BuildingBlocks.EventBus.Abstractions;
using DigitalLearningPlatform.BuildingBlocks.EventBus.Events;

namespace DigitalLearningPlatform.BuildingBlocks.EventBus.Subscriptions
{
    public interface IEventBusSubscriptionManager
    {
        event EventHandler<string> OnEventRemoved;

        void AddDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler;

        void AddSubscription<T, TH>() where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void RemoveDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler;

        void RemoveSubscription<T, TH>() where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        bool HasSubscriptionsForEvent(string eventName);
        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;

        Type? GetEventTypeByName(string eventName);
        void clear();

        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;

        string GetEventKey(Type eventType);
        string GetEventKey<T>();
    }
}
