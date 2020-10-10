using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Clients.Extensions;
using EventHorizon.Identity.AuthServer.Clients.Models;
using EventHorizon.Identity.AuthServer.Clients.Services.Query;
using EventHorizon.Identity.AuthServer.Configuration;
using EventHorizon.Identity.AuthServer.Models.Commands;
using IdentityServer4.EntityFramework.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EventHorizon.Identity.AuthServer.Clients.Services.Create
{
    public class CreateClientCommandHandler
        : IRequestHandler<CreateClientCommand, CommandResult<ClientModel>>
    {
        private readonly IMediator _mediator;
        private readonly HistoryExtendedConfigurationDbContext _configurationDbContext;

        public CreateClientCommandHandler(
            IMediator mediator,
            HistoryExtendedConfigurationDbContext configurationDbContext
        )
        {
            _mediator = mediator;
            _configurationDbContext = configurationDbContext;
        }

        public async Task<CommandResult<ClientModel>> Handle(
            CreateClientCommand request,
            CancellationToken cancellationToken
        )
        {
            if (ClientAlreadyExists(
                request.Client.Id
            ))
            {
                return new CommandResult<ClientModel>(
                    "client_already_exists"
                );
            }
            var entry = await _configurationDbContext.Clients.AddAsync(
                request.Client.ToEntity()
            );
            var client = entry.Entity;

            // Add ClientCorsOrigins
            foreach (var allowedCorsOrigin in request.Client.AllowedCorsOriginList)
            {
                await _configurationDbContext.ClientCorsOrigins.AddAsync(
                    new ClientCorsOrigin
                    {
                        Client = client,
                        Origin = allowedCorsOrigin.CorsOrigin,
                    }
                );
            }

            // Add ClientGrantTypes
            foreach (var grant in request.Client.GrantList)
            {
                await _configurationDbContext.ClientGrantTypes.AddAsync(
                    new ClientGrantType
                    {
                        Client = client,
                        GrantType = grant.GrantTypeSelect,
                    }
                );
            }

            // Add ClientPostLogoutRedirectUris
            foreach (var postLogoutRedirectUri in request.Client.PostLogoutRedirectUriList)
            {
                await _configurationDbContext.ClientPostLogoutRedirectUris.AddAsync(
                    new ClientPostLogoutRedirectUri
                    {
                        Client = client,
                        PostLogoutRedirectUri = postLogoutRedirectUri.PostLogoutRedirectUri,
                    }
                );
            }

            // Add ClientRedirectUris
            foreach (var redirectUri in request.Client.RedirectUriList)
            {
                await _configurationDbContext.ClientRedirectUris.AddAsync(
                    new ClientRedirectUri
                    {
                        Client = client,
                        RedirectUri = redirectUri.RedirectUri,
                    }
                );
            }

            // Add ClientScopes
            foreach (var scope in request.Client.ScopeList)
            {
                await _configurationDbContext.ClientScopes.AddAsync(
                    new ClientScope
                    {
                        Client = client,
                        Scope = scope.Scope,
                    }
                );
            }

            await _configurationDbContext.SaveChangesAsync();

            return await _mediator.Send(
                new QueryForClientById(
                    entry.Entity.ClientId
                )
            );
        }

        private bool ClientAlreadyExists(
            string clientId
        ) => _configurationDbContext.Clients.Count(
            client => client.ClientId == clientId
        ) > 0;
    }
}
