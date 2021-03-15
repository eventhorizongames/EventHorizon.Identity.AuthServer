using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Consent.Create;
using EventHorizon.Identity.AuthServer.Consent.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventHorizon.Identity.AuthServer.Consent.Build
{
    public class BuildConsentViewModelHandler
        : IRequestHandler<BuildConsentViewModel, ConsentViewModel>
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;

        public BuildConsentViewModelHandler(
            ILogger<BuildConsentViewModelHandler> logger,
            IMediator mediator,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IResourceStore resourceStore
        )
        {
            _logger = logger;
            _mediator = mediator;
            _interaction = interaction;
            _clientStore = clientStore;
            _resourceStore = resourceStore;
        }

        public async Task<ConsentViewModel> Handle(
            BuildConsentViewModel requestModel,
            CancellationToken cancellationToken
        )
        {
            var returnUrl = requestModel.ReturnUrl;
            var model = requestModel.Model;

            var request = await _interaction.GetAuthorizationContextAsync(
                returnUrl
            );

            if (request == null)
            {
                _logger.LogError(
                    "No consent request matching request: {0}",
                    returnUrl
                );
                return null;
            }

            var client = await _clientStore.FindEnabledClientByIdAsync(
                request.Client.ClientId
            );
            if (client == null)
            {
                _logger.LogError(
                    "Invalid client id: {0}",
                    request.Client.ClientId
                );
                return null;
            }

            return await _mediator.Send(
                new CreateConsentViewModel(
                    model,
                    returnUrl,
                    request,
                    client,
                    request.ValidatedResources.Resources
                ),
                cancellationToken
            );
        }
    }
}
