using System;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Email.Api;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Email.Set
{
    public struct SetEmailTemplate : IRequest
    {
        public EmailTypes EmailType { get; }
        public string EmailTemplate { get; }
        public SetEmailTemplate(
            EmailTypes emailType,
            string emailTemplate
        )
        {
            EmailType = emailType;
            EmailTemplate = emailTemplate;
        }
        public struct SetEmailTemplateHandler : IRequestHandler<SetEmailTemplate>
        {
            readonly EmailTemplateRepository _repository;
            public SetEmailTemplateHandler(
                EmailTemplateRepository repository
            )
            {
                _repository = repository;
            }
            public Task<Unit> Handle(SetEmailTemplate request, CancellationToken cancellationToken)
            {
                _repository.Set(
                    request.EmailType,
                    request.EmailTemplate
                );
                return Unit.Task;
            }
        }
    }
}