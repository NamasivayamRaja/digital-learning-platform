using DigitalLearningPlatform.BuildingBlocks.EventBus.Abstractions;
using DigitalLearningPlatform.BuildingBlocks.EventBus.Events;
using DigitalLearningPlatform.BuildingBlocks.EventBus.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace DigitalLearningPlatform.BuildingBlocks.EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventBusSubscriptionManager _subscriptionManager;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly int _retryCount;

        public EventBusRabbitMQ(
            IConnection connection,
            IServiceProvider serviceProvider,
            IEventBusSubscriptionManager subscriptionManager,
            ILogger<EventBusRabbitMQ> logger,
            string exchangeName,
            string queueName,
            int retryCount)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _exchangeName = exchangeName;
            _queueName = queueName;
            _retryCount = retryCount;

            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Direct, durable: true);
            _subscriptionManager.OnEventRemoved += OnEventRemoved;
        }

        public Task PublishAsync(IntegrationEvent @event)
        {
            if (!_connection.IsOpen)
            {
                _logger.LogError("Cannot publish event: RabbitMQ connection is not open.");
                throw new InvalidOperationException("RabbitMQ connection is not open");
            }

            var policy = CreateRetryPolicy();

            return policy.ExecuteAsync(() =>
            {
                var eventName = @event.GetType().Name;
                var message = JsonSerializer.Serialize(@event, @event.GetType(), new JsonSerializerOptions { WriteIndented = false });
                var body = Encoding.UTF8.GetBytes(message);

                var properties = _channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // Persistent

                _logger.LogInformation("Publishing event {EventId} to RabbitMQ: {EventName}", @event.Id, eventName);
                _channel.BasicPublish(exchange: _exchangeName, routingKey: eventName, mandatory: true, basicProperties: properties, body: body);
                return Task.CompletedTask;
            });
        }

        public Task SubscribeAsync<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subscriptionManager.GetEventKey<T>();
            DoInternalSubscription(eventName);
            _logger.LogInformation("Subscribing to event {EventName} with {Handler}", eventName, typeof(TH).Name);
            _subscriptionManager.AddSubscription<T, TH>();
            return Task.CompletedTask;
        }
        
        private void DoInternalSubscription(string eventName)
        {
            if (_subscriptionManager.HasSubscriptionsForEvent(eventName)) return;

            var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: $"{_queueName}.{eventName}", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queue: $"{_queueName}.{eventName}", exchange: _exchangeName, routingKey: eventName);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (sender, eventArgs) =>
            {
                try
                {
                    await ProcessEvent(eventName, eventArgs.Body.ToArray());
                    channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing RabbitMQ event: {EventName}", eventName);
                    channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: false);
                }
            };
            
            channel.BasicConsume(queue: $"{_queueName}.{eventName}", autoAck: false, consumer: consumer);
        }
        
        private async Task ProcessEvent(string eventName, byte[] message)
        {
            if (_subscriptionManager.HasSubscriptionsForEvent(eventName))
            {
                using var scope = _serviceProvider.CreateScope();
                var subscriptions = _subscriptionManager.GetHandlersForEvent(eventName);
                foreach (var subscription in subscriptions)
                {
                    var handler = scope.ServiceProvider.GetService(subscription.HandlerType);
                    if (handler == null) continue;

                    var eventType = _subscriptionManager.GetEventTypeByName(eventName);
                    if (eventType == null) 
                    {
                        _logger.LogWarning("No event type found for event name: {EventName}", eventName);
                        continue;
                    }
                    var integrationEvent = JsonSerializer.Deserialize(message, eventType, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                        
                    var handleMethod = concreteType.GetMethod("Handle");
                    if (handleMethod != null && integrationEvent != null)
                    {
                        await (Task)handleMethod.Invoke(handler, new object[] { integrationEvent })!;
                    }
                }
            }
        }
        
        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            _subscriptionManager.RemoveSubscription<T, TH>();
        }

        private void OnEventRemoved(object? sender, string eventName)
        {
            if (!_connection.IsOpen) return;
            using var channel = _connection.CreateModel();
            channel.QueueUnbind(queue: $"{_queueName}.{eventName}", exchange: _exchangeName, routingKey: eventName);
        }
        
        private IAsyncPolicy CreateRetryPolicy()
        {
            return Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetryAsync(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "Could not publish event after {Timeout}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
                });
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}
