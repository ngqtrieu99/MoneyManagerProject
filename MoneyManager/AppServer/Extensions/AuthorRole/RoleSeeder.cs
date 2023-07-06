using AppServer.Common;
using AppServer.Data.Repositories.Token;
using AppServer.Data.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppServer.Extensions.AuthorRole
{
    public interface IRoleSeeder : IDataSeeder
    {
    }

    public class RoleSeeder : IRoleSeeder
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleSeeder(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        public async Task<Response> InitDataBase()
        {
            try
            {
                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                if (!await roleManager.RoleExistsAsync("User"))
                    await roleManager.CreateAsync(new IdentityRole("User"));

                return new Response
                {
                    Status = true,
                    Message = "Create Role successfully"
                };
            }
            catch (Exception e)
            {
                return new Response
                {
                    Status = false,
                    Message = e.Message,
                };
            }
        }
    }
}