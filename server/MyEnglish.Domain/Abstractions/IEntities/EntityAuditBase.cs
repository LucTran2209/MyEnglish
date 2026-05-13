
namespace MyEnglish.Domain.Abstractions.IEntities
{
    public abstract class EntityAuditBase : IUserTracking, IDateTracking, ISoftDelete, IEntityBase
    {
        protected EntityAuditBase()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTimeOffset.UtcNow;
            IsSoftDeleted = false;
        }

        public Guid Id { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
        public bool IsSoftDeleted { get; set; }
    }
}
