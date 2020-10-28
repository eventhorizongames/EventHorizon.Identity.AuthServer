using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Clients.Models;
using EventHorizon.Identity.AuthServer.Configuration;
using EventHorizon.Identity.AuthServer.Models.Commands;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Clients.Services.Query
{
    public class QueryForClientByIdHandler
        : IRequestHandler<QueryForClientById, CommandResult<ClientModel>>
    {
        private readonly HistoryExtendedConfigurationDbContext _configurationDbContext;

        public QueryForClientByIdHandler(
            HistoryExtendedConfigurationDbContext configurationDbContext
        )
        {
            _configurationDbContext = configurationDbContext;
        }

        public Task<CommandResult<ClientModel>> Handle(
            QueryForClientById request, 
            CancellationToken cancellationToken
        )
        {
            if(!_configurationDbContext.Clients.Any(
                a => a.ClientId == request.Id
            ))
            {
                return Task.FromResult(
                    new CommandResult<ClientModel>(
                        "client_not_found"
                    )
                );
            }
            var clientEntity = _configurationDbContext.Clients
                //.Include("AllowedScopes")
                //.Include("AllowedGrantTypes")
                //.Include("ClientSecrets")
                //.Include("RedirectUris")
                //.Include("PostLogoutRedirectUris")
                //.Include("AllowedCorsOrigins")
                //.Include("AllowedScopes")
                .FirstOrDefault(
                    client => client.ClientId == request.Id
                );

            return Task.FromResult(
                new CommandResult<ClientModel>(
                    new ClientModel
                    {
                        Id = request.Id,
                        Name = clientEntity.ClientName,
                    }
                )
            );
        }
    }
}
