using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Email.Api;

namespace EventHorizon.Identity.AuthServer.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(
            EmailTypes type, 
            string email, 
            string subject, 
            string message
        );
    }
}
