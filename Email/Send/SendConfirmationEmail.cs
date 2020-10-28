using MediatR;

namespace EventHorizon.Identity.AuthServer.Email.Send
{
    public partial struct SendConfirmationEmail
        : IRequest
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
    }
}
