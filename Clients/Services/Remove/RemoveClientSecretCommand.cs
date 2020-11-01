using EventHorizon.Identity.AuthServer.Clients.Models;
using EventHorizon.Identity.AuthServer.Models.Commands;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Clients.Services.Remove
{
    public class RemoveClientSecretCommand
        : IRequest<CommandResult<bool>>
    {
        public string ClientId { get; }
        public string Description { get; }

        public RemoveClientSecretCommand(
            string clientId,
            string description
        )
        {
            ClientId = clientId;
            Description = description;
        }
    }
}
