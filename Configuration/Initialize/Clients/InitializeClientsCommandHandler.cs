using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace EventHorizon.Identity.AuthServer.Configuration.Initialize.Clients
{
    public class InitializeClientsCommandHandler 
        : IRequestHandler<InitializeClientsCommand, bool>
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly HistoryExtendedConfigurationDbContext _context;

        public InitializeClientsCommandHandler(
            IHostEnvironment hostEnvironment,
            HistoryExtendedConfigurationDbContext context
        )
        {
            _hostEnvironment = hostEnvironment;
            _context = context;
        }

        public Task<bool> Handle(
            InitializeClientsCommand request, 
            CancellationToken cancellationToken
        )
        {
            if (!_context.Clients.Any())
            {
                foreach (var client in GetClients())
                {
                    _context.Clients.Add(
                        client.ToEntity()
                    );
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
                .SetBasePath(
                    _hostEnvironment.ContentRootPath
                ).AddJsonFile(
                    "clients.json"
                ).AddJsonFile(
                    $"clients.{_hostEnvironment.EnvironmentName}.json", 
                    true
                ).AddEnvironmentVariables()
                .Build();

            // Bind Admin Instance from Config
            var clientConfigFile = new ClientConfigurationFile();
            clientsConfig.Bind(
                clientConfigFile
            );

            return clientConfigFile.Clients.Select(
                client => client.ToEntity()
            );
        }
    }
}
