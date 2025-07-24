using DigitalLearningPlatform.BuildingBlocks.EventBus.Abstractions;
using DigitalLearningPlatform.BuildingBlocks.EventBus.Events;

namespace DigitalLearningPlatform.BuildingBlocks.EventBus.Subscriptions
{
    public class InMemoryEventBusSubscriptionManager : IEventBusSubscriptionManager
    {
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
        private readonly List<Type> _eventTypes;

        public event EventHandler<string>? OnEventRemoved;

        public InMemoryEventBusSubscriptionManager()
        {
            _handlers = new();
            _eventTypes = new();
        }

        public void AddDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            DoAddSubscriptions(typeof(TH),eventName, isDynamic: true);
            _eventTypes.Add(typeof(TH));
        }

        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();
            DoAddSubscriptions(typeof(TH), eventName, isDynamic: false);
            _eventTypes.Add(typeof(T));
        }

        public void clear()
        {
            _handlers.Clear();
            _eventTypes.Clear();
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => 
            _handlers.GetValueOrDefault(eventName, new List<SubscriptionInfo>());

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
        {
            var eventKey = GetEventKey<T>();
            return GetHandlersForEvent(eventKey);
        }

        public string GetEventKey(Type eventType)
        {
            return eventType.Name;
        }

        public string GetEventKey<T>()
        {
            return typeof(T).Name;
        }

        public Type? GetEventTypeByName(string eventName)
        {
            return _eventTypes.SingleOrDefault(e => e.Name == eventName);
        }

        public bool HasSubscriptionsForEvent(string eventName)
        {
            return _handlers.ContainsKey(eventName) && _handlers[eventName].Count > 0;
        }

        public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent
        {
            var eventName = GetEventKey<T>();
            return HasSubscriptionsForEvent(eventName);
        }

        public void RemoveDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            var handlerToRemove = FindDynamicSubscriptionToRemove<TH>(eventName);
            DoRemoveHandler(eventName, handlerToRemove);
        }

        public void RemoveSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var handlerToRemove = FindSubscriptionToRemove<T, TH>();
            var eventName = GetEventKey<T>();
            DoRemoveHandler(eventName, handlerToRemove);
        }

        private void DoAddSubscriptions(Type handlerType, string eventName, bool isDynamic)
        {
            if (HasSubscriptionsForEvent(eventName))
            {
                _handlers.Add(eventName, new List<SubscriptionInfo>());
            }

            if (_handlers[eventName].Any(s=> s.HandlerType == handlerType))
            {
                throw new ArgumentException(
                    $"Handler type {handlerType.Name} already registered for '{eventName}'",
                    nameof(handlerType));
            }

            if(isDynamic)
            {
                _handlers[eventName].Add(SubscriptionInfo.Dynamic(handlerType));
            }
            else
            {
                _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));
            }
        }

        private void DoRemoveHandler(string eventName, SubscriptionInfo? subscriptionInfo)
        {
            if(subscriptionInfo != null)
            {
                _handlers[eventName].Remove(subscriptionInfo);
                var eventType = _eventTypes.SingleOrDefault(t => t.Name == eventName);
                if(eventType != null)
                {
                    _eventTypes.Remove(eventType);
                }
                RaiseOnEventRemoved(eventName);
            }
        }

        private SubscriptionInfo? FindSubscriptionToRemove<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T> 
        {
            var eventName = GetEventKey<T>();

            if (!HasSubscriptionsForEvent(eventName))
            {
                return null;
            }

            return _handlers[eventName].SingleOrDefault(s => s.HandlerType == typeof(TH));
        }

        private SubscriptionInfo? FindDynamicSubscriptionToRemove<TH>(string eventName) where TH: IDynamicIntegrationEventHandler
        {
            if(!HasSubscriptionsForEvent(eventName))
            {
                return null;
            }

            return _handlers[eventName].SingleOrDefault(s => s.HandlerType == typeof(TH) && s.IsDynamic);
        }

        private void RaiseOnEventRemoved(string eventName) {
            OnEventRemoved?.Invoke(this, eventName);
        }
    }
}
