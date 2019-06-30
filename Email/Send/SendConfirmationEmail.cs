using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Email.Api;
using EventHorizon.Identity.AuthServer.Email.Find;
using EventHorizon.Identity.AuthServer.Services;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Email.Send
{
    public struct SendConfirmationEmail : IRequest
    {
        public string Email { get; }
        public string Link { get; }
        public SendConfirmationEmail(
            string email,
            string link
        )
        {
            Email = email;
            Link = link;
        }

        public struct SendConfirmationEmailHandler : IRequestHandler<SendConfirmationEmail>
        {
            readonly IMediator _mediator;
            readonly IEmailSender _emailSender;
            public SendConfirmationEmailHandler(
                IMediator mediator,
                IEmailSender emailSender
            )
            {
                _mediator = mediator;
                _emailSender = emailSender;
            }
            public async Task<Unit> Handle(SendConfirmationEmail request, CancellationToken cancellationToken)
            {
                // Lookup confirmation email template and replace link token, [[CONFIRMATION_LINK]]
                var emailTemplate = await _mediator.Send(
                    new FindEmailTemplate(
                        EmailTypes.CONFIRMATION
                    )
                );
                var confirmationLink = HtmlEncoder.Default.Encode(
                    request.Link
                );
                var emailBody = confirmationLink;
                if (!string.IsNullOrEmpty(emailTemplate))
                {
                    emailBody = emailTemplate.Replace(
                        "[[CONFIRMATION_LINK]]",
                        confirmationLink
                    );
                }
                await _emailSender.SendEmailAsync(
                    EmailTypes.CONFIRMATION,
                    request.Email,
                    "Confirm your email",
                    emailBody
                );
                return Unit.Value;
            }
        }
    }
}