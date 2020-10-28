using System.Collections.Concurrent;
using EventHorizon.Identity.AuthServer.Email.Api;

namespace EventHorizon.Identity.AuthServer.Email.State
{
    public class InMemoryEmailTemplateRepository : EmailTemplateRepository
    {
        private readonly ConcurrentDictionary<EmailTypes, string> STATE = new ConcurrentDictionary<EmailTypes, string>();

        void EmailTemplateRepository.Set(
            EmailTypes type,
            string template
        )
        {
            STATE.AddOrUpdate(
                type,
                template,
                (_, __) => template
            );
        }

        string EmailTemplateRepository.Get(
            EmailTypes type
        )
        {
            if (STATE.TryGetValue(
                type,
                out var template
            ))
            {
                return template;
            }
            return string.Empty;
        }
    }
}
