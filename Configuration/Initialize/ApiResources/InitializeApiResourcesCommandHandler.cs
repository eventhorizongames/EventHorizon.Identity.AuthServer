using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace EventHorizon.Identity.AuthServer.Configuration.Initialize.ApiResources
{
    public class InitializeApiResourcesCommandHandler
        : IRequestHandler<InitializeApiResourcesCommand, bool>
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly HistoryExtendedConfigurationDbContext _context;

        public InitializeApiResourcesCommandHandler(
            IHostEnvironment hostEnvironment,
            HistoryExtendedConfigurationDbContext context
        )
        {
            _hostEnvironment = hostEnvironment;
            _context = context;
        }

        public Task<bool> Handle(
            InitializeApiResourcesCommand request,
            CancellationToken cancellationToken
        )
        {
            if (!_context.ApiResources.Any()
                && !_context.ApiScopes.Any())
            {
                var (apiResourceList, apiScopeList) = GetApiResources();

                foreach (var resource in apiResourceList)
                {
                    _context.ApiResources.Add(
                        resource.ToEntity()
                    );
                }
                foreach (var scope in apiScopeList)
                {
                    _context.ApiScopes.Add(
                        scope.ToEntity()
                    );
                }
                _context.SaveChanges();
            }
            return Task.FromResult(
                true
            );
        }

        private (IEnumerable<ApiResource>, IEnumerable<ApiScope>) GetApiResources()
        {
            var apiResourceConfiguration = new ConfigurationBuilder()
                .SetBasePath(
                    _hostEnvironment.ContentRootPath
                ).AddJsonFile(
                    "api-resources.json"
                ).AddJsonFile(
                    $"api-resources.{_hostEnvironment.EnvironmentName}.json",
                    true
                ).AddEnvironmentVariables()
                .Build();

            var apiResourceFile = new ApiResourceFile();
            apiResourceConfiguration.Bind(
                apiResourceFile
            );

            return (
                apiResourceFile.ApiResources.Select(
                    resource => new ApiResource(
                        resource.Name,
                        resource.DisplayName,
                        resource.ClaimTypes
                    )
                    {
                        Scopes = resource.Scopes
                    }
                ),
                apiResourceFile.ApiResources.SelectMany(
                    a => a.Scopes
                ).Distinct()
                .Select(
                    a => new ApiScope(a)
                )
            );
        }
    }
}
