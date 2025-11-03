namespace MyEnglish.Domain.Abstractions.IEntities
{
    public interface IUserTracking
    {
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
