using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Email.Api;
using EventHorizon.Identity.AuthServer.Services.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace EventHorizon.Identity.AuthServer.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class SaveToFileEmailSender : IEmailSender
    {
        private readonly AuthMessageSenderOptions _authOptions;
        private readonly IHostEnvironment _hostEnvironment;

        public SaveToFileEmailSender(
            IHostEnvironment hostEnvironment,
            IOptions<AuthMessageSenderOptions> optionsAccessor
        )
        {
            _hostEnvironment = hostEnvironment;
            _authOptions = optionsAccessor.Value;
        }

        public Task SendEmailAsync(
            EmailTypes type,
            string email,
            string subject,
            string message
        )
        {
            var emailFile = JsonConvert.SerializeObject(new
            {
                type = Enum.GetName(type.GetType(), type),
                from = $"{_authOptions.FromUserName} <{_authOptions.FromUserEmail}>",
                email,
                subject,
                message
            });
            var directoryPath = Path.Combine(
                _hostEnvironment.ContentRootPath,
                "App_Data",
                "EmailTesting"
            );
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(
                    directoryPath
                );
            }
            using (var fileSystemFile = File.CreateText(
                Path.Combine(
                    directoryPath,
                    this.CreateRandomFileNameWithDate()
                )
            ))
            {
        fileSystemFile.Write(
            emailFile
        );
    }
            return Task.CompletedTask;
    }
    private string CreateRandomFileNameWithDate()
    {
        return $"Testing_Email-{DateTime.Now.Ticks}.json";
    }
}
}
