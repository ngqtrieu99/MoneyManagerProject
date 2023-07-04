using AppServer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppServer.Data.Infrastructure.DbContext
{
    public class ServerDbContext : IdentityDbContext<ApplicationUser>
    {
        public ServerDbContext(DbContextOptions<ServerDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(u =>
            {
                u.HasIndex(p => p.Id).IsUnique(true);
                u.Property(p => p.FirstName).IsRequired();
                u.Property(p => p.LastName).IsRequired();
                u.Property(p => p.CreatedAt).IsRequired();
            });
        }
    }
}