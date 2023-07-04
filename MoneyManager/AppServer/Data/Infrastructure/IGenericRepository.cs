using System.Linq.Expressions;

namespace AppServer.Data.Infrastructure;

public interface IGenericRepository<T> where T : class
{
    // Get with the ID
    Task<T> GetById(Guid id, params string[] includes);
    
    // Get all data
    Task<IEnumerable<T>> GetAll(params string[] includes);
    
    // Get all data with matched conditions
    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> expression);

    Task<T> FindAsync(Expression<Func<T, bool>> match);
    
    // Add the new data
    Task<T> Add(T entity);
    
    // Remove the data
    Task<bool> Remove(Guid id);
    
    // Update the data
    T Update(T entity);
}