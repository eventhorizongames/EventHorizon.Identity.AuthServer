using EventHorizon.Identity.AuthServer.Captcha.Api;
using EventHorizon.Identity.AuthServer.Captcha.Models;
using EventHorizon.Identity.AuthServer.Models.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EventHorizon.Identity.AuthServer.Captcha.Service
{
    public class CaptchaServiceHttpClient
        : CaptchaService
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CaptchaSettings _captchaSettings;

        public CaptchaServiceHttpClient(
            ILogger<CaptchaServiceHttpClient> logger,
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor,
            CaptchaSettings captchaSettings
        )
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _captchaSettings = captchaSettings;
        }

        public async Task<CommandResult<CaptchaValidationResult>> ValidateToken(
            string token,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var remoteIp = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                var content = new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        ["secret"] = _captchaSettings.Secret,
                        ["response"] = token,
                        ["remoteip"] = remoteIp,
                    }
                );
                var clientResponse = await _httpClient.PostAsync(
                    $"/recaptcha/api/siteverify",
                    content,
                    cancellationToken
                );
                return new CommandResult<CaptchaValidationResult>(
                    JsonConvert.DeserializeObject<CaptchaValidationResult>(
                        await clientResponse.Content.ReadAsStringAsync()
                    )
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to validate Captcha token: {CaptchaToken}",
                    token
                );
                return new CommandResult<CaptchaValidationResult>(
                    CaptchaErrorCodes.API_ERROR
                );
            }
        }
    }
}
