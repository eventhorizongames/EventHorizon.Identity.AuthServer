using System.Collections.Concurrent;
using System.Collections.Generic;
using EventHorizon.Identity.AuthServer.Email.Api;

namespace EventHorizon.Identity.AuthServer.Email.State
{
    public class InMemoryEmailTemplateRepository : EmailTemplateRepository
    {
        private ConcurrentDictionary<EmailTypes, string> STATE = new ConcurrentDictionary<EmailTypes, string>();
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
            string template;
            if (STATE.TryGetValue(
                type,
                out template
            ))
            {
                return template;
            }
            return string.Empty;
        }
    }
}