using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Identity.AuthServer.Application
{
    public class ApplicationDbContext 
        : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options
        ): base(
            options
        )
        {
        }

        protected override void OnModelCreating(
            ModelBuilder builder
        )
        {
            base.OnModelCreating(
                builder
            );
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.EnableAutoHistory(
                4000
            );
        }

        public override int SaveChanges(
            bool acceptAllChangesOnSuccess
        )
        {
            this.EnsureAutoHistory();
            return base.SaveChanges(
                acceptAllChangesOnSuccess
            );
        }
        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess, 
            CancellationToken cancellationToken = default
        )
        {
            this.EnsureAutoHistory();
            return base.SaveChangesAsync(
                acceptAllChangesOnSuccess, 
                cancellationToken
            );
        }
    }
}
