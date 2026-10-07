using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DearlerPlatform.Core.Repository
{
    public static class LinqExtensions
    {
        public async static IAsyncEnumerable<TEntity> WhereAsync<TEntity>(this DbSet<TEntity> dbSet, Func<TEntity, bool> predicate)
            where TEntity : class
        {
            var res = dbSet.Where(predicate);
            foreach (var item in res)
            {
                yield return item;
            }
        }

        public async static Task<List<TEntity>> ToListAsync<TEntity>(this IAsyncEnumerable<TEntity> listAsync)
        {
            List<TEntity> list = new();

            await foreach (var item in listAsync)
            {
                list.Add(item);
            }
            return list;
        }
    }
}