using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Application;
using EventHorizon.Identity.AuthServer.Services.Claims;
using EventHorizon.Identity.AuthServer.Services.Role;
using EventHorizon.Identity.AuthServer.Services.User;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace EventHorizon.Identity.AuthServer.Admins.Create
{
    public class CreateAdminsCommandHandler
        : IRequestHandler<CreateAdminsCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _applicationDbContext;

        public CreateAdminsCommandHandler(
            IMediator mediator,
            IHostEnvironment hostEnvironment,
            ApplicationDbContext applicationDbContext
        )
        {
            _mediator = mediator;
            _hostEnvironment = hostEnvironment;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<bool> Handle(
            CreateAdminsCommand request,
            CancellationToken cancellationToken
        )
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(
                    _hostEnvironment.ContentRootPath
                ).AddJsonFile(
                    "admins.json"
                ).AddJsonFile(
                    $"admins.{_hostEnvironment.EnvironmentName}.json",
                    true
                ).AddEnvironmentVariables()
                .Build();

            // Bind Admin Instance from Config
            var admins = new AdminUserConfiguration();
            config.Bind(
                admins
            );

            // Create Admin Roles
            await _mediator.Send(
                new RoleCreateEvent
                {
                    RoleName = UserRoles.ADMIN
                }
            );
            await _mediator.Send(
                new RoleAddClaimEvent
                {
                    RoleName = UserRoles.ADMIN,
                    Claim = new Claim(
                        IdentityClaimTypes.PERMISSION,
                        "identity.create"
                    ),
                }
            );
            await _mediator.Send(
                new RoleAddClaimEvent
                {
                    RoleName = UserRoles.ADMIN,
                    Claim = new Claim(
                        IdentityClaimTypes.PERMISSION,
                        "identity.update"
                    ),
                }
            );
            await _mediator.Send(
                new RoleAddClaimEvent
                {
                    RoleName = UserRoles.ADMIN,
                    Claim = new Claim(
                        IdentityClaimTypes.PERMISSION,
                        "identity.view"
                    ),
                }
            );
            foreach (var admin in admins.Admins)
            {
                var adminUser = _applicationDbContext
                    .Users
                    .FirstOrDefault(
                        user => user.Email == admin.Email
                    );
                if (adminUser == null)
                {
                    var result = await _mediator.Send(
                        new UserCreateEvent
                        {
                            User = new Models.ApplicationUser
                            {
                                Id = admin.Id ?? System.Guid.NewGuid().ToString(),
                                UserName = admin.Email,
                                Email = admin.Email,
                            },
                            Profile = new Models.ApplicationUserProfile
                            {

                            },
                            Password = admin.Password,
                        }
                    );
                    if (result.Succeeded)
                    {
                        adminUser = _applicationDbContext
                            .Users
                            .FirstOrDefault(
                                user => user.Email == admin.Email
                            );
                    }
                    else
                    {
                        continue;
                    }
                }
                await _mediator.Publish(
                    new UserAddToRoleEvent
                    {
                        User = adminUser,
                        Role = UserRoles.ADMIN
                    }
                );
            }
            return true;
        }
    }
}
