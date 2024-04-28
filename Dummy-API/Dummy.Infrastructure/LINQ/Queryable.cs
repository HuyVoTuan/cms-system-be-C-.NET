using System.Linq.Expressions;

namespace Dummy.Infrastructure.LINQ
{
    public static class Queryable
    {
        public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int pageIndex, int pageLimit)
        {
            return source.Skip((pageIndex - 1) * pageLimit).Take(pageLimit);
        }

        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }
    }
}
