namespace DigitalLearningPlatform.BuildingBlocks.Common.Domain
{
    public class AuditableEntityBase : EntityBase
    {
        public DateTime CreatedAt { get; protected set; }
        public Guid CreatedById { get; protected set; }
        public DateTime? LastModifiedAt { get; protected set; }
        public Guid LastModifiedById { get; protected set;}

        public AuditableEntityBase() : base() 
        {
            CreatedAt = DateTime.UtcNow;
            CreatedById = SystemActors.SystemUserId;
        }

        public void UpdateModificationAudit(Guid modifiedById)
        {
            LastModifiedAt = DateTime.UtcNow;
            LastModifiedById = modifiedById;
        }
    }
}
