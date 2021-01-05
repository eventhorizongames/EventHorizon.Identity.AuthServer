using EventHorizon.Identity.AuthServer.Captcha.Api;
using EventHorizon.Identity.AuthServer.Captcha.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventHorizon.Identity.AuthServer.Captcha.Validate
{
    public class ValidateCaptchaCommandHandler
        : IRequestHandler<ValidateCaptchaCommand, CaptchaValidationResult>
    {
        private readonly CaptchaService _captchaService;

        public ValidateCaptchaCommandHandler(
            CaptchaService captchaService
        )
        {
            _captchaService = captchaService;
        }
        public async Task<CaptchaValidationResult> Handle(
            ValidateCaptchaCommand request,
            CancellationToken cancellationToken
        )
        {
            var validatedToken = await _captchaService.ValidateToken(
                request.Token,
                cancellationToken
            );
            if (validatedToken.Success)
            {
                return validatedToken.Result;
            }
            return new CaptchaValidationResult
            {
                Success = false,
                ErrorCodes = new string[] { validatedToken.ErrorCode }
            };
        }
    }
}
