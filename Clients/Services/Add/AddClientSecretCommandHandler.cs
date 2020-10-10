using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Clients.Models;
using EventHorizon.Identity.AuthServer.Clients.Services.Query;
using EventHorizon.Identity.AuthServer.Configuration;
using EventHorizon.Identity.AuthServer.Models.Commands;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Clients.Services.Add
{
    public class AddClientSecretCommandHandler
        : IRequestHandler<AddClientSecretCommand, CommandResult<ClientModel>>
    {
        private readonly IMediator _mediator;
        private readonly HistoryExtendedConfigurationDbContext _configurationDbContext;

        public AddClientSecretCommandHandler(
            IMediator mediator,
            HistoryExtendedConfigurationDbContext configurationDbContext
        )
        {
            _mediator = mediator;
            _configurationDbContext = configurationDbContext;
        }

        public async Task<CommandResult<ClientModel>> Handle(
            AddClientSecretCommand request, 
            CancellationToken cancellationToken
        )
        {
            var clientId = request.ClientId;
            var secretValue = System.Text.Encoding.UTF8.GetString(
                Convert.FromBase64String(
                    request.Secret.Value
                )
            );
            if (!_configurationDbContext.ClientSecrets.Any(
                a => a.Client.ClientId == clientId && a.Value == secretValue.Sha256()
            ))
            {
                await _configurationDbContext.ClientSecrets.AddAsync(
                    new ClientSecret
                    {
                        Client = _configurationDbContext.Clients.First(a => a.ClientId == clientId),
                        Description = request.Secret.Description,
                        Value = secretValue.Sha256(),
                    }
                );
                await _configurationDbContext.SaveChangesAsync();
            }

            return await _mediator.Send(
                new QueryForClientById(
                    clientId
                )
            );
        }
    }
}
