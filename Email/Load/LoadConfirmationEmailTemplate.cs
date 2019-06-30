using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Email.Api;
using MediatR;
using Microsoft.AspNetCore.Hosting;

namespace EventHorizon.Identity.AuthServer.Email.Load
{
    public struct LoadConfirmationEmailTemplate : IRequest
    {
        public struct LoadConfirmationEmailTemplateHandler : IRequestHandler<LoadConfirmationEmailTemplate>
        {
            readonly IHostingEnvironment _hostingEnvironment;
            readonly EmailTemplateRepository _repository;
            public LoadConfirmationEmailTemplateHandler(
                IHostingEnvironment hostingEnvironment,
                EmailTemplateRepository repository
            )
            {
                _hostingEnvironment = hostingEnvironment;
                _repository = repository;
            }
            public Task<Unit> Handle(
                LoadConfirmationEmailTemplate request,
                CancellationToken cancellationToken
            )
            {
                var pathToFile = Path.Combine(
                    _hostingEnvironment.ContentRootPath,
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
}