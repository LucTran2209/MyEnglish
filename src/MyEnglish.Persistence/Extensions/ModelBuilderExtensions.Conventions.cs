using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq;

namespace MyEnglish.Persistence.Extensions
{
    public static partial class ModelBuilderExtensions
    {
        public static ModelBuilder ConfigureConvention(
        this ModelBuilder builder,
        Func<IMutableProperty, bool> condition,
        Action<PropertyBuilder> action)
        {
            foreach (var property in builder.Model.GetEntityTypes()
                                        .SelectMany(x => x.GetProperties())
                                        .Where(condition)
                                        .Select(p => builder.Entity(p.DeclaringType.ClrType).Property(p.Name)))
            {
                action(property);
            }

            return builder;
        }

        public static ModelBuilder ConfigureConvention(
            this ModelBuilder builder,
            Func<IMutableProperty, bool> condition,
            Action<IMutableProperty> action)
        {
            foreach (var property in builder.Model.GetEntityTypes()
                                        .SelectMany(x => x.GetProperties())
                                        .Where(condition))
            {
                action(property);
            }

            return builder;
        }

        public static ModelBuilder ConfigureConventions(this ModelBuilder builder)
        {
            builder.ConfigureConvention(
                x => x.ClrType == typeof(DateTime) || x.ClrType == typeof(DateTime?),
                x => x.HasColumnType("datetime2"));

            builder.ConfigureConvention(
               x => x.Name.ToLower() == "version",
               b => b.ValueGeneratedOnAddOrUpdate()
                   .IsRowVersion()
                   .IsConcurrencyToken());

            builder.ConfigureConvention(
                x => x.ClrType == typeof(decimal) || x.ClrType == typeof(decimal?),
                b => b.HasPrecision(18, 2));

            return builder;
        }
    }
}
