using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Application;
using EventHorizon.Identity.AuthServer.Services.Claims;
using EventHorizon.Identity.AuthServer.Services.Role;
using EventHorizon.Identity.AuthServer.Services.User;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace EventHorizon.Identity.AuthServer.Admins.Create
{
    public struct CreateAdminsHandler : IRequestHandler<CreateAdminsCommand, bool>
    {
        readonly IMediator _mediator;
        readonly IHostingEnvironment _hostingEnvironment;
        readonly ApplicationDbContext _applicationDbContext;

        public CreateAdminsHandler(
            IMediator mediator,
            IHostingEnvironment hostingEnvironment,
            ApplicationDbContext applicationDbContext
        )
        {
            _mediator = mediator;
            _hostingEnvironment = hostingEnvironment;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<bool> Handle(CreateAdminsCommand request, CancellationToken cancellationToken)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(_hostingEnvironment.ContentRootPath)
                .AddJsonFile("admins.json")
                .AddJsonFile($"admins.{_hostingEnvironment.EnvironmentName}.json", true)
                .AddEnvironmentVariables()
                .Build();

            // Bind Admin Instance from Config
            var admins = new AdminUserConfiguration();
            config.Bind(admins);

            // Create Admin Roles
            await _mediator.Send(new RoleCreateEvent
            {
                RoleName = UserRoles.ADMIN
            });
            await _mediator.Send(new RoleAddClaimEvent
            {
                RoleName = UserRoles.ADMIN,
                Claim = new Claim(IdentityClaimTypes.PERMISSION, "identity.create"),
            });
            await _mediator.Send(new RoleAddClaimEvent
            {
                RoleName = UserRoles.ADMIN,
                Claim = new Claim(IdentityClaimTypes.PERMISSION, "identity.update"),
            });
            await _mediator.Send(new RoleAddClaimEvent
            {
                RoleName = UserRoles.ADMIN,
                Claim = new Claim(IdentityClaimTypes.PERMISSION, "identity.view"),
            });
            foreach (var admin in admins.Admins)
            {
                var adminUser = _applicationDbContext.Users.FirstOrDefault(a => a.Email == admin.Email);
                if (adminUser == null)
                {
                    var result = await _mediator.Send(new UserCreateEvent
                    {
                        User = new Models.ApplicationUser
                        {
                            UserName = admin.Email,
                            Email = admin.Email,
                        },
                        Profile = new Models.ApplicationUserProfile
                        {

                        },
                        Password = admin.Password,
                    });
                    if (result.Succeeded)
                    {
                        adminUser = _applicationDbContext.Users.FirstOrDefault(a => a.Email == admin.Email);
                    }
                    else
                    {
                        continue;
                    }
                }
                await _mediator.Publish(new UserAddToRoleEvent
                {
                    User = adminUser,
                    Role = UserRoles.ADMIN
                });
            }
            return true;
        }
    }
}