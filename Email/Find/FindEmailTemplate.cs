using EventHorizon.Identity.AuthServer.Email.Api;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Email.Find
{
    public struct FindEmailTemplate 
        : IRequest<string>
    {
        public EmailTypes EmailType { get; }

        public FindEmailTemplate(
            EmailTypes emailType
        )
        {
            EmailType = emailType;
        }
    }
}
