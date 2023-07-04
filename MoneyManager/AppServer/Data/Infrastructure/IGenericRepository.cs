using System.Linq.Expressions;

namespace AppServer.Data.Infrastructure;

public interface IGenericRepository<T> where T : class
{
    // Get with the ID
    Task<T> GetById(Guid id);
    
    // Get all data
    Task<IEnumerable<T>> GetAll(params string[] includes);
    
    // Get all data with matched conditions
    IEnumerable<T> Find(Expression<Func<T, bool>> expression);
    
    // Add the new data
    Task<T> Add(T entity);
    
    // Remove the data
    Task<T> Remove(Guid id);
    
    // Update the data
    Task<T> Update(T entity);
}