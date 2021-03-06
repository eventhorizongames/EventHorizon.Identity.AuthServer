using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Email.Api;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace EventHorizon.Identity.AuthServer.Email.Load
{
    public class LoadConfirmationEmailTemplateHandler 
        : IRequestHandler<LoadConfirmationEmailTemplate>
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly EmailTemplateRepository _repository;

        public LoadConfirmationEmailTemplateHandler(
            IHostEnvironment hostEnvironment,
            EmailTemplateRepository repository
        )
        {
            _hostEnvironment = hostEnvironment;
            _repository = repository;
        }

        public Task<Unit> Handle(
            LoadConfirmationEmailTemplate request,
            CancellationToken cancellationToken
        )
        {
            var pathToFile = Path.Combine(
                _hostEnvironment.ContentRootPath,
                "App_Data",
                "EmailTemplates",
                "Confirmation.html"
            );

            if (!File.Exists(
                pathToFile
            ))
            {
                throw new Exception(
                    "Confirmation.html email template not found."
                );
            }

            _repository.Set(
                EmailTypes.CONFIRMATION,
                File.ReadAllText(
                    pathToFile
                )
            );

            return Unit.Task;
        }
    }
}
