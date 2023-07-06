using AppServer.Data.Infrastructure.DbContext;

namespace AppServer.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ServerDbContext _dbContext;

    public UnitOfWork(ServerDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task CommitAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}