using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Configuration.Initialize.ApiResources
{
    public struct InitializeApiResourcesHandler : IRequestHandler<InitializeApiResourcesCommand, bool>
    {
        readonly HistoryExtendedConfigurationDbContext _context;

        public InitializeApiResourcesHandler(
            HistoryExtendedConfigurationDbContext context
        )
        {
            _context = context;
        }

        public Task<bool> Handle(InitializeApiResourcesCommand request, CancellationToken cancellationToken)
        {
            if (!_context.ApiResources.Any())
            {
                foreach (var resource in GetApiResources())
                {
                    _context.ApiResources.Add(resource.ToEntity());
                }
                _context.SaveChanges();
            }
            return Task.FromResult(
                true
            );
        }
        private static IEnumerable<ApiResource> GetApiResources()
        {
            // TODO: Load this from a file
            return new ApiResource[]
            {
                new ApiResource("api1", "My API #1"),
                new ApiResource("roles", "Role", new[] {"role"})
            };
        }
    }
}