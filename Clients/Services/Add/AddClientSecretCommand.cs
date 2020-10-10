using System;
using EventHorizon.Identity.AuthServer.Clients.Models;
using EventHorizon.Identity.AuthServer.Models.Commands;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Clients.Services.Add
{
    public class AddClientSecretCommand
        : IRequest<CommandResult<ClientModel>>
    {
        public string ClientId { get; }
        public AddSecretModel Secret { get; }

        public AddClientSecretCommand(
            string clientId,
            AddSecretModel secret
        )
        {
            ClientId = clientId;
            Secret = secret;
        }
    }
}
