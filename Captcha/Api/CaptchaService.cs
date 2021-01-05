using EventHorizon.Identity.AuthServer.Captcha.Models;
using EventHorizon.Identity.AuthServer.Models.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventHorizon.Identity.AuthServer.Captcha.Api
{
    public interface CaptchaService
    {
        Task<CommandResult<CaptchaValidationResult>> ValidateToken(
            string token,
            CancellationToken cancellationToken
        );
    }
}
