using EventHorizon.Identity.AuthServer.Models.Commands;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Clients.Services.Remove
{
    public class RemoveClientCommand
        : IRequest<CommandResult<bool>>
    {
        public string ClientId { get; }

        public RemoveClientCommand(
            string clientId
        )
        {
            ClientId = clientId;
        }
    }
}
