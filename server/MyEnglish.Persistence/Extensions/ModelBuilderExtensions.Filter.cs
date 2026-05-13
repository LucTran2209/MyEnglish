using Microsoft.EntityFrameworkCore;
using MyEnglish.Domain.Abstractions.IEntities;
using System.Reflection;

namespace MyEnglish.Persistence.Extensions
{
    public static partial class ModelBuilderExtensions
    {
        static readonly MethodInfo IsDeletedFilterMethod = typeof(ModelBuilderExtensions)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Single(builder => builder.IsGenericMethod && builder.Name == "IsDeletedFilter");

        public static void IsDeletedFilter(this ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                //TODO: If code does not work, check this condition (maybe change codition to only "typeof(IsSoftDelete).IsAssignableFrom(entityType.ClrType)")
                if (entityType.ClrType.GetProperty("IsSoftDeleted") != null &&
                    entityType.ClrType.GetProperty("IsSoftDeleted")?.PropertyType == typeof(bool) &&
                    typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    IsDeletedFilterMethod
                        .MakeGenericMethod(entityType.ClrType)
                        .Invoke(null, new object[] { builder });
                }
            }
        }

        private static void IsDeletedFilter<TEntity>(ModelBuilder modelBuilder)
            where TEntity : class, ISoftDelete
        {
            modelBuilder.Entity<TEntity>().HasQueryFilter(x => !x.IsSoftDeleted);
        }
    }
}
