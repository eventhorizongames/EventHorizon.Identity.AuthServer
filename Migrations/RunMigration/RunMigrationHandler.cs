using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Application;
using EventHorizon.Identity.AuthServer.Configuration;
using IdentityServer4.EntityFramework.DbContexts;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Identity.AuthServer.Migrations.RunMigration
{
    public struct RunMigrationHandler : IRequestHandler<RunMigrationCommand, bool>
    {
        readonly IHostingEnvironment _hostingEnvironment;
        readonly ApplicationDbContext _applicationDbContext;
        readonly PersistedGrantDbContext _persistedGrantDbContext;
        readonly HistoryExtendedConfigurationDbContext _historyExtendedConfigurationDbContext;

        public RunMigrationHandler(
            IHostingEnvironment hostingEnvironment,
            ApplicationDbContext applicationDbContext,
            PersistedGrantDbContext persistedGrantDbContext,
            HistoryExtendedConfigurationDbContext historyExtendedConfigurationDbContext
        )
        {
            _hostingEnvironment = hostingEnvironment;
            _applicationDbContext = applicationDbContext;
            _persistedGrantDbContext = persistedGrantDbContext;
            _historyExtendedConfigurationDbContext = historyExtendedConfigurationDbContext;
        }

        public Task<bool> Handle(RunMigrationCommand request, CancellationToken cancellationToken)
        {
            _applicationDbContext
                .Database
                .Migrate();

            _persistedGrantDbContext
                .Database
                .Migrate();

            _historyExtendedConfigurationDbContext
                .Database
                .Migrate();
            return Task.FromResult(true);
        }
    }
}