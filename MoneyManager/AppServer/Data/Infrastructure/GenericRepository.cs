using System.Linq.Expressions;
using AppServer.Data.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace AppServer.Data.Infrastructure;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ServerDbContext dbContext;

    protected DbSet<T> dbSet;

    public GenericRepository(ServerDbContext dbContext)
    {
        this.dbContext = dbContext;
        dbSet = dbContext.Set<T>();
    }

    public async Task<T> Add(T entity)
    {
        await dbSet.AddAsync(entity);
        return entity;
    }

    public async Task<IEnumerable<T>> GetAll(params string[] includes)
    {
        IQueryable<T> query = dbSet;
        if (includes.Length > 0)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }
        return query;
    }

    public async Task<T> GetById(Guid id, params string[] includes)
    {
        var model = await dbSet.FindAsync(id);
        foreach (var path in includes)
        {
            dbContext.Entry(model).Reference(path).LoadAsync();
        }
        return model;
    }

    public T Update(T entity)
    {
        dbSet.Update(entity);
        return entity;
    }

    public async Task<bool> Remove(Guid id)
    {
        var entityRemove = await dbSet.FindAsync(id);

        if(entityRemove != null)
        {
            dbSet.Remove(entityRemove);
            return true;
        }

        return false;
    }

    public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> expression)
    {
        return await dbSet.Where(expression).ToListAsync();
    }
    
    public async Task<T> FindAsync(Expression<Func<T, bool>> match)
    {
        return await dbSet.SingleOrDefaultAsync(match);
    }
}