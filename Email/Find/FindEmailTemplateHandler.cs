using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Email.Api;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Email.Find
{
    public class FindEmailTemplateHandler 
        : IRequestHandler<FindEmailTemplate, string>
    {
        private readonly EmailTemplateRepository _repository;

        public FindEmailTemplateHandler(
            EmailTemplateRepository repository
        )
        {
            _repository = repository;
        }

        public Task<string> Handle(
            FindEmailTemplate request, 
            CancellationToken cancellationToken
        )
        {
            return Task.FromResult(
                _repository.Get(
                    request.EmailType
                )
            );
        }
    }
}
