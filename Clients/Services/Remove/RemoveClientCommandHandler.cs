using EventHorizon.Identity.AuthServer.Configuration;
using EventHorizon.Identity.AuthServer.Models.Commands;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventHorizon.Identity.AuthServer.Clients.Services.Remove
{
    public class RemoveClientCommandHandler
        : IRequestHandler<RemoveClientCommand, CommandResult<bool>>
    {
        private readonly HistoryExtendedConfigurationDbContext _dbContext;

        public RemoveClientCommandHandler(
            HistoryExtendedConfigurationDbContext dbContext
        )
        {
            _dbContext = dbContext;
        }

        public async Task<CommandResult<bool>> Handle(
            RemoveClientCommand request,
            CancellationToken cancellationToken
        )
        {
            var entity = _dbContext.Clients
                .FirstOrDefault(
                    a => a.ClientId == request.ClientId
                );
            if (entity != null)
            {
                _dbContext.Clients.Remove(
                    entity
                );
                await _dbContext.SaveChangesAsync();
            }

            return new CommandResult<bool>(
                true
            );
        }
    }
}
