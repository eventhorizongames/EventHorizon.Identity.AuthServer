namespace EventHorizon.Identity.AuthServer.Services.Models
{
    public class AuthMessageSenderOptions
    {
        public string FromUserEmail { get; set; }
        public string FromUserName { get; set; }
        public string ApiKey { get; set; }
    }
}