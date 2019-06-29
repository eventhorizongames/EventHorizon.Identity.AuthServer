using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Services.Models;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace EventHorizon.Identity.AuthServer.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        readonly AuthMessageSenderOptions Options;
        public EmailSender(
            IOptions<AuthMessageSenderOptions> optionsAccessor
        )
        {
            Options = optionsAccessor.Value;
        }

        public Task SendEmailAsync(
            string type, 
            string email, 
            string subject, 
            string message
        )
        {
            return Execute(
                Options.ApiKey, 
                subject, 
                message, 
                email,
                Options.FromUserEmail,
                Options.FromUserName
            );
        }

        public Task Execute(
            string apiKey, 
            string subject, 
            string message, 
            string email,
            string fromUserEmail,
            string fromUserName
        )
        {
            var client = new SendGridClient(
                apiKey
            );
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(
                    fromUserEmail, fromUserName
                ),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // We do not want to track reset password and confirmation email links.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }
    }
}
