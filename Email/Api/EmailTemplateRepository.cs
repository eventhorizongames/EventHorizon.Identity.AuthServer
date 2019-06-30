namespace EventHorizon.Identity.AuthServer.Email.Api
{
    public interface EmailTemplateRepository
    {
        void Set(
            EmailTypes type,
            string template
        );
        string Get(
            EmailTypes type
        );
    }
}