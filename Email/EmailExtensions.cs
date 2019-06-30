using EventHorizon.Identity.AuthServer.Email.Load;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace EventHorizon.Identity.AuthServer
{
    public static class EmailExtensions
    {
        public static IApplicationBuilder AddEmailExtensions(
            this IApplicationBuilder app
        )
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider
                    .GetService<IMediator>()
                    .Send(
                        new LoadPasswordResetEmailTemplate()
                    )
                    .GetAwaiter().GetResult();
                serviceScope.ServiceProvider
                    .GetService<IMediator>()
                    .Send(
                        new LoadConfirmationEmailTemplate()
                    )
                    .GetAwaiter().GetResult();
            }
            return app;
        }
    }
}