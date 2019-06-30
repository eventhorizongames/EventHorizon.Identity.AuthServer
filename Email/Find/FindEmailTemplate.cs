using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Email.Api;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Email.Find
{
    public struct FindEmailTemplate : IRequest<string>
    {
        public EmailTypes EmailType { get; }
        public FindEmailTemplate(
            EmailTypes emailType
        )
        {
            EmailType = emailType;
        }

        public struct FindEmailTemplateHandler : IRequestHandler<FindEmailTemplate, string>
        {
            readonly EmailTemplateRepository _repository;
            public FindEmailTemplateHandler(
                EmailTemplateRepository repository
            )
            {
                _repository = repository;
            }
            public Task<string> Handle(FindEmailTemplate request, CancellationToken cancellationToken)
            {
                return Task.FromResult(
                    _repository.Get(
                        request.EmailType
                    )
                );
            }
        }
    }
}