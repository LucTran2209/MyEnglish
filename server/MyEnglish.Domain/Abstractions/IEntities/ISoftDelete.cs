namespace MyEnglish.Domain.Abstractions.IEntities
{
    public interface ISoftDelete
    {
        public bool IsSoftDeleted { get; set; }
    }
}
