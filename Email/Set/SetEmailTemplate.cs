using System;
using EventHorizon.Identity.AuthServer.Email.Api;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Email.Set
{
    public struct SetEmailTemplate
        : IRequest
    {
        public EmailTypes EmailType { get; }
        public string EmailTemplate { get; }

        public SetEmailTemplate(
            EmailTypes emailType,
            string emailTemplate
        )
        {
            EmailType = emailType;
            EmailTemplate = emailTemplate;
        }
    }
}
