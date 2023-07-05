using AppServer.Models;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace AppServer.Extensions
{
    public static class SeederExtension
    {
        public static IApplicationBuilder InitSeeder(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var initializer = serviceScope.ServiceProvider.GetServices<IDataSeeder>();

            foreach (var init in initializer)
            {
                init.InitDataBase();
            }

            return app;
        }
    }
}