using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Email.Api;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Email.Set
{
    public class SetEmailTemplateHandler
        : IRequestHandler<SetEmailTemplate>
    {
        private readonly EmailTemplateRepository _repository;

        public SetEmailTemplateHandler(
            EmailTemplateRepository repository
        )
        {
            _repository = repository;
        }

        public Task<Unit> Handle(
            SetEmailTemplate request,
            CancellationToken cancellationToken
        )
        {
            _repository.Set(
                request.EmailType,
                request.EmailTemplate
            );
            return Unit.Task;
        }
    }
}
