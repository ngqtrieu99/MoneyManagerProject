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

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(u =>
            {
                //u.HasIndex(p => p.Id).IsUnique(true);
                u.Property(p => p.Email).IsRequired(true);
                u.Property(p => p.Email).IsRequired(true);
                u.Property(p => p.FirstName).IsRequired(true);
                u.Property(p => p.LastName).IsRequired(true);
                u.Property(p => p.CreatedAt).IsRequired(true);
            });
        }
    }
}