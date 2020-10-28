using System.Linq;
using EventHorizon.Identity.AuthServer.Users.Services.Fetch.Model;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Users.Services.Fetch
{
    public struct QueryForListOfUsers
        : IRequest<IQueryable<QueryUser>>
    {
    }
}
