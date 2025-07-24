using DigitalLearningPlatform.BuildingBlocks.EventBus.Events;

namespace DigitalLearningPlatform.BuildingBlocks.EventBus.Abstractions
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IEventHandler where TIntegrationEvent : IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }
}
