using EventHorizon.Identity.AuthServer.Clients.Models;
using EventHorizon.Identity.AuthServer.Models.Commands;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Clients.Services.Create
{
    public struct CreateClientCommand
        : IRequest<CommandResult<ClientModel>>
    {
        public ClientCreateModel Client { get; }

        public CreateClientCommand(
            ClientCreateModel client
        )
        {
            Client = client;
        }
    }
}
