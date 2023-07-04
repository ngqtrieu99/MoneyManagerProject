namespace AppServer.Data.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    Task CommitAsync();
}