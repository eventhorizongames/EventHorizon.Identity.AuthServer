using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Models;
using EventHorizon.Identity.AuthServer.Users.Services.Fetch.Model;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EventHorizon.Identity.AuthServer.Users.Services.Fetch
{
    public struct QueryForListOfUsers : IRequest<IQueryable<QueryUser>>
    {
    }

    public struct QueryForListOfUsersHandler : IRequestHandler<QueryForListOfUsers, IQueryable<QueryUser>>
    {
        readonly UserManager<ApplicationUser> _userManager;
        public QueryForListOfUsersHandler(
            UserManager<ApplicationUser> userManager
        )
        {
            _userManager = userManager;
        }
        public Task<IQueryable<QueryUser>> Handle(
            QueryForListOfUsers request, 
            CancellationToken cancellationToken
        )
        {
            return Task.FromResult(
                _userManager.Users.OrderBy(
                    user => user.Id
                ).OrderBy(
                    user => user.Id
                ).Select(
                    user => new QueryUser
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email
                    }
                )
            );
        }
    }
}