using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Admins.Create;
using EventHorizon.Identity.AuthServer.Application;
using EventHorizon.Identity.AuthServer.Configuration;
using EventHorizon.Identity.AuthServer.Configuration.Initialize.ApiResources;
using EventHorizon.Identity.AuthServer.Configuration.Initialize.AuthIdentityResources;
using EventHorizon.Identity.AuthServer.Configuration.Initialize.Clients;
using EventHorizon.Identity.AuthServer.Migrations.RunMigration;
using EventHorizon.Identity.AuthServer.Services.Claims;
using EventHorizon.Identity.AuthServer.Services.Role;
using EventHorizon.Identity.AuthServer.Services.User;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventHorizon.Identity.AuthServer
{
    public static class AuthDatabase
    {
        public static void InitializeDatabase(
            IServiceProvider services,
            bool runMigration
        )
        {
            using (var serviceScope = services.GetService<IServiceScopeFactory>().CreateScope())
            {
                var mediator = serviceScope.ServiceProvider.GetRequiredService<IMediator>();
                if (runMigration)
                {
                    serviceScope.MigrateContexts(mediator)
                        .GetAwaiter().GetResult();
                }
                serviceScope.CreateAdmins(mediator)
                    .GetAwaiter().GetResult();
                serviceScope.CreateConfiguration(mediator)
                    .GetAwaiter().GetResult();
            }
        }

        private static async Task MigrateContexts(this IServiceScope serviceScope, IMediator mediator)
        {
            await mediator.Send(
                new RunMigrationCommand()
            );
        }
        private static async Task CreateAdmins(this IServiceScope serviceScope, IMediator mediator)
        {
            await mediator.Send(
                new CreateAdminsCommand()
            );
        }

        private static async Task CreateConfiguration(this IServiceScope serviceScope, IMediator mediator)
        {
            await mediator.Send(
                new InitializeClientsCommand()
            );
            await mediator.Send(
                new InitializeIdentityResourcesCommand()
            );
            await mediator.Send(
                new InitializeApiResourcesCommand()
            );
        }
    }
}