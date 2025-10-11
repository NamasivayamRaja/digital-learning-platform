using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace DigitalLearningPlatform.BuildingBlocks.EventBus.RabbitMQ
{
    /// <summary>
    /// Health check for RabbitMQ connection status
    /// Essential for enterprise monitoring and alerting
    /// </summary>
    public class RabbitMQHealthCheck : IHealthCheck
    {
        private readonly IConnection _connection;

        public RabbitMQHealthCheck(IConnection connection)
        {
            _connection = connection;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                if (_connection.IsOpen)
                {
                    return Task.FromResult(HealthCheckResult.Healthy("RabbitMQ connection is open"));
                }
                
                return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ connection is closed"));
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ connection check failed", ex));
            }
        }
    }
}