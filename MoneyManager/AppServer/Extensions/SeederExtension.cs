using Microsoft.AspNetCore.Identity;
using System.Data;

namespace AppServer.Extensions
{
    public static class SeederExtension
    {
        public static async Task InitSeeder(this IApplicationBuilder builder)
        {
            //     using var serviceScope = app.ApplicationServices.CreateScope();
            //
            //     var initializer = serviceScope.ServiceProvider.GetServices<IDataSeeder>();
            //
            //     foreach (var init in initializer)
            //     {
            //         init.InitDataBase();
            //     }
            //
            //     return app;
            // }

            using var scope = builder.ApplicationServices.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string admin = "Admin";
            if (!(await roleManager.RoleExistsAsync(admin)))
            {
                await roleManager.CreateAsync(new IdentityRole(admin));
            }

            string user = "User";
            if (!(await roleManager.RoleExistsAsync(user)))
            {
                await roleManager.CreateAsync(new IdentityRole(user));
            }
        }
    }
}