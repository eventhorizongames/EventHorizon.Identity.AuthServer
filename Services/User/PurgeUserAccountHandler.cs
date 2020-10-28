using System;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace EventHorizon.Identity.AuthServer.Services.User
{
    public class PurgeUserAccountHandler
        : IRequestHandler<PurgeUserAccount, bool>
    {
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public PurgeUserAccountHandler(
            ILogger<PurgeUserAccountHandler> logger,
            UserManager<ApplicationUser> userManager
        )
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<bool> Handle(
            PurgeUserAccount request,
            CancellationToken cancellationToken
        )
        {
            try
            {
                await _userManager.DeleteAsync(
                    request.User
                );
            }
            catch (Exception ex)
            {
                // Log the user that was going to be purged.
                // TODO: Create a backup log of emails that need to be purged later.
                _logger.LogError(
                    ex,
                    "Failed to delete User Email: {}", request.User?.Email
                );
                return false;
            }
            return true;
        }
    }
}
