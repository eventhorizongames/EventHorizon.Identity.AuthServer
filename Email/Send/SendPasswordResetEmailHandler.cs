using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Email.Api;
using EventHorizon.Identity.AuthServer.Email.Find;
using EventHorizon.Identity.AuthServer.Services;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Email.Send
{
    public class SendPasswordResetEmailHandler 
        : IRequestHandler<SendPasswordResetEmail>
    {
        private readonly IMediator _mediator;
        private readonly IEmailSender _emailSender;

        public SendPasswordResetEmailHandler(
            IMediator mediator,
            IEmailSender emailSender
        )
        {
            _mediator = mediator;
            _emailSender = emailSender;
        }

        public async Task<Unit> Handle(
            SendPasswordResetEmail request,
            CancellationToken cancellationToken
        )
        {
            // Lookup confirmation email template and replace link token, [[PASSWORD_RESET_LINK]]
            var emailTemplate = await _mediator.Send(
                new FindEmailTemplate(
                    EmailTypes.PASSWORD_RESET
                )
            );
            var passwordResetLink = HtmlEncoder.Default.Encode(
                request.Link
            );
            var emailBody = passwordResetLink;
            if (!string.IsNullOrEmpty(emailTemplate))
            {
                emailBody = emailTemplate.Replace(
                    "[[PASSWORD_RESET_LINK]]",
                    passwordResetLink
                );
            }

            await _emailSender.SendEmailAsync(
                EmailTypes.PASSWORD_RESET,
                request.Email,
                // TODO: Localize this from the request.Locale
                "Confirm your email",
                emailBody
            );
            return Unit.Value;
        }
    }
}
