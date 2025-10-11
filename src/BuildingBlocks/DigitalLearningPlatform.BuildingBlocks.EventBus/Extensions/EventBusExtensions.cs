using DigitalLearningPlatform.BuildingBlocks.EventBus.Abstractions;
using DigitalLearningPlatform.BuildingBlocks.EventBus.RabbitMQ;
using DigitalLearningPlatform.BuildingBlocks.EventBus.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace DigitalLearningPlatform.BuildingBlocks.EventBus.Extensions
{
    public static class EventBusExtensions
    {
        public static IServiceCollection AddEventBusRabbitMQ(
            this IServiceCollection services,
            string connectionString,
            string serviceName,
            int retryCount = 3)
        {
            services.AddSingleton<IConnectionFactory>(sp =>
            {
                var factory = new ConnectionFactory();
                if (Uri.TryCreate(connectionString, UriKind.Absolute, out var uri))
                {
                    factory.Uri = uri;
                }
                else
                {
                    factory.HostName = connectionString;
                }
                factory.AutomaticRecoveryEnabled = true;
                factory.DispatchConsumersAsync = true; // Required for AsyncEventingBasicConsumer
                return factory;
            });

            services.AddSingleton<IConnection>(sp =>
            {
                var factory = sp.GetRequiredService<IConnectionFactory>();
                return factory.CreateConnection($"{serviceName}-EventBus");
            });

            services.AddSingleton<IEventBusSubscriptionManager, InMemoryEventBusSubscriptionManager>();

            services.AddSingleton<IEventBus>(sp =>
            {
                var connection = sp.GetRequiredService<IConnection>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var subscriptionManager = sp.GetRequiredService<IEventBusSubscriptionManager>();
                
                return new EventBusRabbitMQ(connection, sp, subscriptionManager, logger, "digital_learning_platform", serviceName, retryCount);
            });
            
            // Add the health check registration within the same extension method
            services.AddHealthChecks().AddCheck<RabbitMQHealthCheck>("rabbitmq");

            return services;
        }
    }
}
