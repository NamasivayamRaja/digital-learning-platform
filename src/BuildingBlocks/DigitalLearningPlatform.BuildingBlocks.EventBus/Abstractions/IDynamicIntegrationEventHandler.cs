namespace DigitalLearningPlatform.BuildingBlocks.EventBus.Abstractions
{
    public interface IDynamicIntegrationEventHandler : IEventHandler
    {
        Task Handle(dynamic eventData);
    }
}
