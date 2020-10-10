
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Clients.Models;
using EventHorizon.Identity.AuthServer.Models.Commands;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Clients.Services.Query
{
    public struct QueryForClientById
        : IRequest<CommandResult<ClientModel>>
    {
        public string Id { get; }

        public QueryForClientById(
            string id
        )
        {
            Id = id;
        }
    }
}
