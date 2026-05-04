using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyEnglish.Persistence.DataSeeds;

namespace MyEnglish.Persistence.Extensions
{
    public static class BuilderExtensison
    {
        public static IApplicationBuilder MigrationDataBase(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<DbContexts.SqlServerDbContext>();

            if (context == null)
            {
                throw new InvalidOperationException("DbContext is not registered.");
            }

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            var contributors = serviceScope.ServiceProvider.GetServices<IDataSeedContributor>();

            foreach (var contributor in contributors)
            {
                contributor.SeedAsync().GetAwaiter().GetResult();
            }

            return app;
        }

    }
}
