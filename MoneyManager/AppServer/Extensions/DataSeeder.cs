using AppServer.Common;
using AppServer.Extensions.AuthorRole;

namespace AppServer.Extensions
{
    public interface IDataSeeder
    {
        Task<Response> InitDataBase();
    }

    public class DataSeeder : IDataSeeder, IRoleSeeder
    {
        private readonly ILogger<DataSeeder> logger;

        private readonly IRoleSeeder roleSeeder;

        public DataSeeder(ILogger<DataSeeder> logger, IRoleSeeder roleSeeder)
        {
            this.logger = logger;
            this.roleSeeder = roleSeeder;
        }

        public async Task<Response> InitDataBase()
        {
            try
            {
                await roleSeeder.InitDataBase();

                return new Response
                {
                    Status = true,
                    Message = "Successfully"
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }
    }
}