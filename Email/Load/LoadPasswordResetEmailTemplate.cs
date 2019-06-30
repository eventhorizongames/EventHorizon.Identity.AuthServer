using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Email.Api;
using MediatR;
using Microsoft.AspNetCore.Hosting;

namespace EventHorizon.Identity.AuthServer.Email.Load
{
    public struct LoadPasswordResetEmailTemplate : IRequest
    {
        public struct LoadPasswordResetEmailTemplateHandler : IRequestHandler<LoadPasswordResetEmailTemplate>
        {
            readonly IHostingEnvironment _hostingEnvironment;
            readonly EmailTemplateRepository _repository;
            public LoadPasswordResetEmailTemplateHandler(
                IHostingEnvironment hostingEnvironment,
                EmailTemplateRepository repository
            )
            {
                _hostingEnvironment = hostingEnvironment;
                _repository = repository;
            }
            public Task<Unit> Handle(
                LoadPasswordResetEmailTemplate request,
                CancellationToken cancellationToken
            )
            {
                var pathToFile = Path.Combine(
                    _hostingEnvironment.ContentRootPath,
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
}