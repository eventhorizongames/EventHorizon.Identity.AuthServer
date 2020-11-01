using EventHorizon.Identity.AuthServer.Configuration;
using EventHorizon.Identity.AuthServer.Models.Commands;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventHorizon.Identity.AuthServer.Clients.Services.Remove
{
    public class RemoveClientSecretCommandHandler
        : IRequestHandler<RemoveClientSecretCommand, CommandResult<bool>>
    {
        private readonly HistoryExtendedConfigurationDbContext _dbContext;

        public RemoveClientSecretCommandHandler(
            HistoryExtendedConfigurationDbContext dbContext
        )
        {
            _dbContext = dbContext;
        }

        public async Task<CommandResult<bool>> Handle(
            RemoveClientSecretCommand request, 
            CancellationToken cancellationToken
        )
        {
            var secret = _dbContext.ClientSecrets
                .FirstOrDefault(
                    clientSecret => clientSecret.Client.ClientId == request.ClientId 
                        && clientSecret.Description == request.Description
                );
            if (secret != null)
            {
                _dbContext.ClientSecrets.Remove(
                    secret
                );
                await _dbContext.SaveChangesAsync();
            }

            return new CommandResult<bool>(
                true
            );
        }
    }
}
