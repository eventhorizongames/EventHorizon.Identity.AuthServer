using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace EventHorizon.Identity.AuthServer.Configuration.Initialize.Clients
{
    public struct InitializeClientsHandler : IRequestHandler<InitializeClientsCommand, bool>
    {
        readonly IHostingEnvironment _hostingEnvironment;
        readonly HistoryExtendedConfigurationDbContext _context;

        public InitializeClientsHandler(
            IHostingEnvironment hostingEnvironment,
            HistoryExtendedConfigurationDbContext context
        )
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        public Task<bool> Handle(InitializeClientsCommand request, CancellationToken cancellationToken)
        {
            if (!_context.Clients.Any())
            {
                foreach (var client in GetClients())
                {
                    _context.Clients.Add(client.ToEntity());
                }
                _context.SaveChanges();
            }
            return Task.FromResult(
                true
            );
        }

        private IEnumerable<Client> GetClients()
        {
            // Load configuration from a file
            var clientsConfig = new ConfigurationBuilder()
                .SetBasePath(_hostingEnvironment.ContentRootPath)
                .AddJsonFile("clients.json")
                .AddJsonFile($"clients.{_hostingEnvironment.EnvironmentName}.json", true)
                .AddEnvironmentVariables()
                .Build();

            // Bind Admin Instance from Config
            var clientConfigFile = new ClientConfigurationFile();
            clientsConfig.Bind(clientConfigFile);

            return clientConfigFile.Clients.Select(client => client.ToEntity());
        }
    }
}