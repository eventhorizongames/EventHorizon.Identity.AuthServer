using EventHorizon.Identity.AuthServer.Captcha.Models;
using EventHorizon.Identity.AuthServer.Models.Commands;
using MediatR;
using System;

namespace EventHorizon.Identity.AuthServer.Captcha.Validate
{
    public class ValidateCaptchaCommand
        : IRequest<CaptchaValidationResult>
    {
        public string Token { get; }

        public ValidateCaptchaCommand(
            string token
        )
        {
            Token = token;
        }
    }
}
