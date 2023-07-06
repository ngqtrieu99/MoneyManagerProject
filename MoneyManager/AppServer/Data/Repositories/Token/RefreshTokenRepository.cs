using AppServer.Data.Infrastructure;
using AppServer.Data.Infrastructure.DbContext;
using AppServer.DataModels;

namespace AppServer.Data.Repositories.Token
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> FindRefreshToken(string token);

        Task<RefreshToken> AddToken(RefreshToken token);

        RefreshToken UpdateToken(RefreshToken token);
    }

    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ServerDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken> AddToken(RefreshToken token)
        {
            return await Add(token);
        }

        public async Task<RefreshToken> FindRefreshToken(string token)
        {
            return await FindAsync(element => element.Token.Equals(token));
        }

        public RefreshToken UpdateToken(RefreshToken token)
        {
            return Update(token);
        }
    }
}