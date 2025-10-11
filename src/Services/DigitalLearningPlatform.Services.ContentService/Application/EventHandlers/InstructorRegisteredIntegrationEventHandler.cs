using DigitalLearningPlatform.BuildingBlocks.EventBus.Abstractions;
using DigitalLearningPlatform.BuildingBlocks.MessageContracts;
using DigitalLearningPlatform.Services.ContentService.Domain;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace DigitalLearningPlatform.Services.ContentService.Application.EventHandlers
{
    /// <summary>
    /// Enterprise event handler for InstructorRegisteredIntegrationEvent
    /// Implements idempotent processing, comprehensive logging, and error handling
    /// Follows the saga pattern for distributed transactions
    /// </summary>
    public class InstructorRegisteredIntegrationEventHandler : IIntegrationEventHandler<InstructorRegisteredIntegrationEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InstructorRegisteredIntegrationEventHandler> _logger;

        public InstructorRegisteredIntegrationEventHandler(
            IUnitOfWork unitOfWork,
            ILogger<InstructorRegisteredIntegrationEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(InstructorRegisteredIntegrationEvent @event)
        {
            _logger.LogInformation(
                "Processing InstructorRegisteredIntegrationEvent for instructor {UserId} - {FullName} (EventId: {EventId})",
                @event.UserId, @event.FullName, @event.Id);

            try
            {
                // Begin transaction for consistency
                await _unitOfWork.BeginTransactionAsync();

                // Check if author already exists (idempotency check)
                var existingAuthor = await _unitOfWork.AuthorLookUps.GetByAuthorIdAsync(@event.UserId);
                
                if (existingAuthor != null)
                {
                    _logger.LogInformation(
                        "Author lookup already exists for instructor {UserId}. Updating existing record (EventId: {EventId})",
                        @event.UserId, @event.Id);
                        
                    // Update existing record with latest information
                    existingAuthor.AuthorName = @event.FullName;
                    await _unitOfWork.AuthorLookUps.UpdateAsync(existingAuthor);
                }
                else
                {
                    _logger.LogInformation(
                        "Creating new author lookup for instructor {UserId} - {FullName} (EventId: {EventId})",
                        @event.UserId, @event.FullName, @event.Id);

                    // Create new AuthorLookUp record
                    var authorLookup = new AuthorLookUp
                    {
                        AuthorId = @event.UserId,
                        AuthorName = @event.FullName
                    };

                    await _unitOfWork.AuthorLookUps.AddAsync(authorLookup);
                }

                // Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation(
                    "Successfully processed InstructorRegisteredIntegrationEvent for instructor {UserId} - {FullName} (EventId: {EventId})",
                    @event.UserId, @event.FullName, @event.Id);

                // Optional: Publish domain events or trigger additional business processes
                await PublishAuthorCreatedDomainEventAsync(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to process InstructorRegisteredIntegrationEvent for instructor {UserId} - {FullName} (EventId: {EventId}). Error: {ErrorMessage}",
                    @event.UserId, @event.FullName, @event.Id, ex.Message);

                // Rollback transaction
                await _unitOfWork.RollbackTransactionAsync();

                // Re-throw to trigger retry mechanism in RabbitMQ
                // The event will be retried according to the retry policy
                // and eventually sent to dead letter queue if all retries fail
                throw;
            }
        }

        /// <summary>
        /// Publishes domain events for downstream processes
        /// This could trigger notifications, analytics, or other business processes
        /// </summary>
        private async Task PublishAuthorCreatedDomainEventAsync(InstructorRegisteredIntegrationEvent @event)
        {
            try
            {
                // Log structured data for analytics and monitoring
                _logger.LogInformation(
                    "Author created successfully: {AuthorId} - {AuthorName} | Registration Date: {RegistrationDate} | Event Timestamp: {EventTimestamp}",
                    @event.UserId,
                    @event.FullName,
                    @event.RegistrationDate,
                    @event.CreationDate);

                // In a full enterprise implementation, you might:
                // 1. Publish domain events to trigger other processes
                // 2. Update search indexes (Elasticsearch)
                // 3. Send welcome emails to instructors
                // 4. Update analytics dashboards
                // 5. Trigger ML/AI pipelines for instructor recommendations

                await Task.CompletedTask; // Placeholder for future implementations
            }
            catch (Exception ex)
            {
                // Log but don't fail the main process
                _logger.LogWarning(ex,
                    "Failed to publish domain events for author {AuthorId}, but main process succeeded",
                    @event.UserId);
            }
        }
    }
}