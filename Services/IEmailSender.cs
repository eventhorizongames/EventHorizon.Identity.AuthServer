using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHorizon.Identity.AuthServer.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string type, string email, string subject, string message);
    }
}
