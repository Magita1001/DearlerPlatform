using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DearlerPlatform.Core.Core;
using DearlerPlatform.Core.Global;
using Microsoft.EntityFrameworkCore;
namespace DearlerPlatform.Core.Repository;



// public class Repository<TEntity> : IRepository where TEntity : BaseEntity
public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly DealerPlatformContext _dbContext;

    public Repository(DealerPlatformContext dbContext)
    {
        this._dbContext = dbContext;         
    }
    public List<TEntity> GetList()
    {
        //和以前_dbContext.Video.AddAsync()的方式相比，直接通过泛型得到 dbSet 的方式更加方便
        // return _context.Set<TEntity>().ToList();

        var dbSet = _dbContext.Set<TEntity>();
        return dbSet.ToList();
    }
    public List<TEntity> GetList(Func<TEntity, bool> predicate)
    {
        var dbSet = _dbContext.Set<TEntity>();
        return dbSet.Where(predicate).AsQueryable().ToList();
    }
    public async Task<List<TEntity>> GetListAsync()
    {
        // return await GetListAsync("ProductName", 1, 30);
        return await GetListAsync(new PageWithSortDto()
        {
            Sort = "Id"
        });

        // var dbSet = _dbContext.Set<TEntity>();
        // return await dbSet.ToListAsync();
    }
    // public async Task<List<TEntity>> GetListAsync(string sort, int pageIndex, int pagesize)
    public async Task<List<TEntity>> GetListAsync(PageWithSortDto pageWithSortDto)
    {
        int skip = (pageWithSortDto.PageIndex - 1) * pageWithSortDto.PageSize;

        var dbSet = _dbContext.Set<TEntity>();
        if (pageWithSortDto.OrderType == OrderType.Acs)
        {
            return await dbSet.OrderBy(pageWithSortDto.Sort).Skip(skip).Take(pageWithSortDto.PageSize).ToListAsync();
        }
        else
        {
            return await dbSet.OrderByDescending(pageWithSortDto.Sort).Skip(skip).Take(pageWithSortDto.PageSize).ToListAsync();
        }
    }
    public IQueryable<TEntity> GetQueryble()
    {
        var dbSet = _dbContext.Set<TEntity>();
        return dbSet;
    }
    //拓展写法 WhereAsync
    // public async Task<List<TEntity>> GetListAsync(Func<TEntity, bool> predicate)
    // {
    //     var dbSet = _dbContext.Set<TEntity>();
    //     return await dbSet.WhereAsync(predicate).ToListAsync();
    // }

    //正式的表达式目录树的写法
    public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var dbSet = _dbContext.Set<TEntity>();
        return await dbSet.Where(predicate).ToListAsync();
    }
    public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, string sort, int pageIndex, int pagesize)
    {
        int skip = (pageIndex - 1) * pagesize;
        var dbSet = _dbContext.Set<TEntity>();
        return await dbSet.Where(predicate).OrderBy(m => m.GetType().GetProperty(sort).GetValue(m)).Skip(skip).Take(pagesize).ToListAsync();
    }

    //查找相关方法
    public TEntity Get(Func<TEntity, bool> predicate)
    {
        var dbSet = _dbContext.Set<TEntity>();
        return dbSet.FirstOrDefault(predicate);
    }
    public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var dbSet = _dbContext.Set<TEntity>();
        return await dbSet.FirstOrDefaultAsync(predicate);
    }


    //插入相关方法
    public TEntity Insert(TEntity entity)
    {
        var dbSet = _dbContext.Set<TEntity>();

        var res = dbSet.Add(entity).Entity;
        _dbContext.SaveChanges();

        return res;
    }
    public async Task<TEntity> InsertAsync(TEntity entity)
    {
        var dbSet = _dbContext.Set<TEntity>();

        //在SaveChangesAsync执行后 Entity里是从数据库取回的，带有Id的正式数据 这里仅作为查看插入内容情况
        var res = (await dbSet.AddAsync(entity)).Entity;
        await _dbContext.SaveChangesAsync();

        return res;
    }

    //删除相关方法
    public TEntity Delate(TEntity entity)
    {
        var dbSet = _dbContext.Set<TEntity>();

        var res = dbSet.Remove(entity).Entity;
        _dbContext.SaveChanges();

        return res;
    }
    public async Task<TEntity> DelateAsync(TEntity entity)
    {
        var dbSet = _dbContext.Set<TEntity>();
        var res = dbSet.Remove(entity).Entity;
        await _dbContext.SaveChangesAsync();

        return res;
    }

    //增加相关方法
    public TEntity Update(TEntity entity)
    {
        var dbSet = _dbContext.Set<TEntity>();

        var res = dbSet.Update(entity).Entity;
        _dbContext.SaveChanges();

        return res;
    }
    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        var dbSet = _dbContext.Set<TEntity>();
        var res = dbSet.Update(entity).Entity;
        await _dbContext.SaveChangesAsync();

        return res;
    }
}
