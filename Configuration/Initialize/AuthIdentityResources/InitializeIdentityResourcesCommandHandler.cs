using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Configuration.Initialize.AuthIdentityResources
{
    public class InitializeIdentityResourcesCommandHandler
        : IRequestHandler<InitializeIdentityResourcesCommand, bool>
    {
        private readonly HistoryExtendedConfigurationDbContext _context;

        public InitializeIdentityResourcesCommandHandler(
            HistoryExtendedConfigurationDbContext context
        )
        {
            _context = context;
        }

        public Task<bool> Handle(
            InitializeIdentityResourcesCommand request,
            CancellationToken cancellationToken
        )
        {
            if (!_context.IdentityResources.Any())
            {
                foreach (var resource in GetIdentityResources())
                {
                    _context.IdentityResources.Add(
                        resource.ToEntity()
                    );
                }
                _context.SaveChanges();
            }
            return Task.FromResult(
                true
            );
        }

        private static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }
    }
}
