using Microsoft.EntityFrameworkCore;
using MyEnglish.Application.Common.PagingModels;
using MyEnglish.Domain.Abstractions.IEntities;
using System.Linq.Expressions;

namespace MyEnglish.Application.Extensions
{
    public static class DataExtensions
    {
        public static IQueryable<T> ToPageList<T>(this IQueryable<T> query, PagedRequest filter)
        where T : EntityAuditBase
        {
            int pageIndex = filter.PageNumberIndex ?? 0;
            pageIndex = pageIndex <= 1 ? 0 : pageIndex - 1;

            int pageSize = filter.PageSize ?? 10;
            query = query.Skip(pageSize * pageIndex)
                .Take(pageSize);
            return query;
        }

        public static async Task<PagedResult<T>> ToPageResultAsync<T>(this IQueryable<T> query, int count, PagedRequest filter)
            where T : class
        {
            return new PagedResult<T>
            {
                Items = await query.ToListAsync(),
                PageNumber = filter.PageNumberIndex ?? 1,
                PageSize = filter.PageSize ?? 10,
                TotalPages = (int)Math.Ceiling(count / (double)(filter.PageSize ?? 10)),
                TotalItems = count,
            };
        }

        public static async Task<PagedResult<Y>> ToPageResultAsync<T, Y>(
            this IQueryable<T> query,
            int count,
            PagedRequest filter,
            Expression<Func<T, Y>> mapping)
            where T : EntityAuditBase
            where Y : class
        {
            return new PagedResult<Y>
            {
                Items = await query.Select(mapping).ToListAsync(),
                PageNumber = filter.PageNumberIndex ?? 1,
                PageSize = filter.PageSize ?? 10,
                TotalPages = (int)Math.Ceiling(count / (double)(filter.PageSize ?? 10)),
                TotalItems = count,
            };
        }

        public static IQueryable<T> Filter<T>(this IQueryable<T> query, string? filter, Expression<Func<T, bool>> predicate)
            where T : EntityAuditBase
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return query;
            }
            return query.Where(predicate);
        }

        public static IQueryable<T> Filter<T>(this IQueryable<T> query, int? filter, Expression<Func<T, bool>> predicate)
            where T : EntityAuditBase
        {
            if (filter == null)
            {
                return query;
            }
            return query.Where(predicate);
        }
        public static IQueryable<T> Filter<T>(this IQueryable<T> query, long? filter, Expression<Func<T, bool>> predicate)
            where T : EntityAuditBase
        {
            if (filter == null)
            {
                return query;
            }
            return query.Where(predicate);
        }

        public static IQueryable<T> Filter<T>(this IQueryable<T> query, decimal? filter, Expression<Func<T, bool>> predicate)
            where T : EntityAuditBase
        {
            if (filter == null)
            {
                return query;
            }
            return query.Where(predicate);
        }

        public static async Task<T?> GetById<T, Tkey>(this IQueryable<T> query, Tkey id)
            where T : IEntityBase
        {
            return await query.FirstOrDefaultAsync(x => x.Id.ToString() == id!.ToString());
        }
    }
}
