using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Email.Api;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace EventHorizon.Identity.AuthServer.Email.Load
{
    public class LoadPasswordResetEmailTemplateHandler 
        : IRequestHandler<LoadPasswordResetEmailTemplate>
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly EmailTemplateRepository _repository;

        public LoadPasswordResetEmailTemplateHandler(
            IHostEnvironment hostEnvironment,
            EmailTemplateRepository repository
        )
        {
            _hostEnvironment = hostEnvironment;
            _repository = repository;
        }

        public Task<Unit> Handle(
            LoadPasswordResetEmailTemplate request,
            CancellationToken cancellationToken
        )
        {
            var pathToFile = Path.Combine(
                _hostEnvironment.ContentRootPath,
                "App_Data",
                "EmailTemplates",
                "ResetPassword.html"
            );

            if (!File.Exists(
                pathToFile
            ))
            {
                throw new Exception(
                    "RestPassword.html email template not found."
                );
            }

            _repository.Set(
                EmailTypes.PASSWORD_RESET,
                File.ReadAllText(
                    pathToFile
                )
            );
            return Unit.Task;
        }
    }
}
