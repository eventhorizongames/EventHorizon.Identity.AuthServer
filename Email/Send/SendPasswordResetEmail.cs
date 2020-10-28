using MediatR;

namespace EventHorizon.Identity.AuthServer.Email.Send
{
    public struct SendPasswordResetEmail
        : IRequest
    {
        public string Email { get; }
        public string Link { get; }

        public SendPasswordResetEmail(
            string email,
            string link
        )
        {
            Email = email;
            Link = link;
        }
    }
}
