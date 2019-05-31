using System.Linq;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Configuration;
using EventHorizon.Identity.AuthServer.Models;
using EventHorizon.Identity.AuthServer.Users.Fetch;
using EventHorizon.Identity.AuthServer.Users.Models;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventHorizon.Identity.AuthServer.Users
{
    [Authorize("Identity Admin")]
    [Route("[controller]")]
    public class UserController : Controller
    {
        readonly IMediator _mediator;
        readonly UserManager<ApplicationUser> _userManager;
        public UserController(
            IMediator mediator,
            UserManager<ApplicationUser> userManager
        )
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            [FromQuery] int page = 1,
            [FromQuery] int take = 1
        )
        {
            var model = new UserViewModel
            {
                Count = _userManager.Users.Count()
            };
            foreach (var user in (await _mediator.Send(
                    new QueryForListOfUsers()
                )).Skip(
                    (page - 1) * take
                ).Take(
                    take
                ).Select(user =>
                    new UserDisplayModel
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email
                    }
                )
            )
            {
                model.Users.Add(
                    user
                );
            }

            return View(model);
        }
    }
}