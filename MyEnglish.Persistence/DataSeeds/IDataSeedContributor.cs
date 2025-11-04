namespace MyEnglish.Persistence.DataSeeds
{
    public interface IDataSeedContributor
    {
        Task SeedAsync();
    }
}
