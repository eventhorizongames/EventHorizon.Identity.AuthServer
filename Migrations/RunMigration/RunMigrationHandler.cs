using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Application;
using EventHorizon.Identity.AuthServer.Configuration;
using IdentityServer4.EntityFramework.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Identity.AuthServer.Migrations.RunMigration
{
    public class RunMigrationHandler
        : IRequestHandler<RunMigrationCommand, bool>
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly PersistedGrantDbContext _persistedGrantDbContext;
        private readonly HistoryExtendedConfigurationDbContext _historyExtendedConfigurationDbContext;

        public RunMigrationHandler(
            ApplicationDbContext applicationDbContext,
            PersistedGrantDbContext persistedGrantDbContext,
            HistoryExtendedConfigurationDbContext historyExtendedConfigurationDbContext
        )
        {
            _applicationDbContext = applicationDbContext;
            _persistedGrantDbContext = persistedGrantDbContext;
            _historyExtendedConfigurationDbContext = historyExtendedConfigurationDbContext;
        }

        public Task<bool> Handle(
            RunMigrationCommand request,
            CancellationToken cancellationToken
        )
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

            return Task.FromResult(
                true
            );
        }
    }
}
