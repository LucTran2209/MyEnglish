using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyEnglish.Persistence.DbContexts;

namespace MyEnglish.Persistence.DependencyInjections
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var sqlConnectionString = configuration.GetConnectionString("SqlServerConnection") ?? throw new InvalidOperationException("Connection string 'SqlServerConnection' not found.");

            services.AddDbContext<SqlServerDbContext>((provider, options) =>
            {
                options.UseSqlServer(sqlConnectionString, option =>
                {
                    option.MigrationsHistoryTable("_MigrationsHistory");
                    option.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            });

            //services.AddDatabaseDeveloperPageExceptionFilter();

            //Seed Data
            //services.AddTransient<IDataSeedContributor, UsersDataSeedContributor>();
            
            return services;
        }
    }
}
