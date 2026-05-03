using System.Linq.Expressions;

namespace MyEnglish.Application.Extensions
{
    public static class QueryableExtensions
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string? propertyName, bool orderByDesc = false, IComparer<object>? comparer = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                // Todo cast IOrderedQueryable but extra CPU
                return query.OrderBy(p => 0);
            }

            string methordName = orderByDesc ? nameof(Enumerable.OrderByDescending) : nameof(Enumerable.OrderBy);
            return CallOrderedQueryable(query, methordName, propertyName, comparer);
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> query, string? propertyName, bool thenByDesc = false, IComparer<object>? comparer = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return query;
            }

            string methordName = thenByDesc ? nameof(Enumerable.ThenByDescending) : nameof(Enumerable.ThenBy);
            return CallOrderedQueryable(query, methordName, propertyName, comparer);
        }

        public static IOrderedQueryable<T> CallOrderedQueryable<T>(this IQueryable<T> query, string methodName, string propertyName,
                IComparer<object>? comparer = null)
        {
            var param = Expression.Parameter(typeof(T), "x");

            var body = propertyName.Split('.').Aggregate<string, Expression>(param, Expression.PropertyOrField);

            return comparer != null
                ? (IOrderedQueryable<T>)query.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        methodName,
                        new[] { typeof(T), body.Type },
                        query.Expression,
                        Expression.Lambda(body, param),
                        Expression.Constant(comparer)
                    )
                )
                : (IOrderedQueryable<T>)query.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        methodName,
                        new[] { typeof(T), body.Type },
                        query.Expression,
                        Expression.Lambda(body, param)
                    )
                );
        }
    }
}
