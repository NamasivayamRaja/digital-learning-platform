using DigitalLearningPlatform.BuildingBlocks.EventBus.Events;
using System.Threading.Tasks;

namespace DigitalLearningPlatform.BuildingBlocks.EventBus.Abstractions
{
    public interface IEventBus
    {
        Task PublishAsync(IntegrationEvent @event);

        Task SubscribeAsync<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
    }
}
