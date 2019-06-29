using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Consent.Build;
using EventHorizon.Identity.AuthServer.Consent.Models;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventHorizon.Identity.AuthServer.Consent.Processing
{
    public struct ProcessPageConsentRequest : IRequest<ProcessConsentResult>
    {
        public ConsentInputModel Model { get; set; }
        public ClaimsPrincipal User { get; set; }

        public ProcessPageConsentRequest(
            ConsentInputModel model,
            ClaimsPrincipal user
        )
        {
            Model = model;
            User = user;
        }

        public struct ProcessPageConsentRequestHandler : IRequestHandler<ProcessPageConsentRequest, ProcessConsentResult>
        {
            private readonly IMediator _mediator;
            private readonly IIdentityServerInteractionService _interaction;
            private readonly IEventService _events;

            public ProcessPageConsentRequestHandler(
                IMediator mediator,
                IIdentityServerInteractionService interaction,
                IEventService events
            )
            {
                _mediator = mediator;
                _interaction = interaction;
                _events = events;
            }

            public async Task<ProcessConsentResult> Handle(
                ProcessPageConsentRequest eventRequest,
                CancellationToken cancellationToken
            )
            {
                var model = eventRequest.Model;
                var user = eventRequest.User;
                var result = new ProcessConsentResult();

                // validate return url is still valid
                var request = await _interaction.GetAuthorizationContextAsync
                (
                    model.ReturnUrl
                );
                if (request == null) return result;

                ConsentResponse grantedConsent = null;

                // user clicked 'no' - send back the standard 'access_denied' response
                if (model.Button == "no")
                {
                    grantedConsent = ConsentResponse.Denied;

                    // emit event
                    await _events.RaiseAsync
                    (
                        new ConsentDeniedEvent
                        (
                            user.GetSubjectId(),
                            result.ClientId,
                            request.ScopesRequested
                        )
                    );
                }
                // user clicked 'yes' - validate the data
                else if (model.Button == "yes"
                    && model != null
                )
                {
                    // if the user consented to some scope, build the response model
                    if (model.ScopesConsented != null
                        && model.ScopesConsented.Any()
                    )
                    {
                        var scopes = model.ScopesConsented;
                        if (ConsentOptions.EnableOfflineAccess == false)
                        {
                            scopes = scopes.Where
                            (
                                x => x != IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess
                            );
                        }

                        grantedConsent = new ConsentResponse
                        {
                            RememberConsent = model.RememberConsent,
                            ScopesConsented = scopes.ToArray()
                        };

                        // emit event
                        await _events.RaiseAsync
                        (
                            new ConsentGrantedEvent
                            (
                                user.GetSubjectId(),
                                request.ClientId,
                                request.ScopesRequested,
                                grantedConsent.ScopesConsented,
                                grantedConsent.RememberConsent
                            )
                        );
                    }
                    else
                    {
                        result.ValidationError = ConsentOptions.MustChooseOneErrorMessage;
                    }
                }
                else
                {
                    result.ValidationError = ConsentOptions.InvalidSelectionErrorMessage;
                }

                if (grantedConsent != null)
                {
                    // communicate outcome of consent back to identityserver
                    await _interaction.GrantConsentAsync
                    (
                        request,
                        grantedConsent
                    );

                    // indicate that's it ok to redirect back to authorization endpoint
                    result.RedirectUri = model.ReturnUrl;
                    result.ClientId = request.ClientId;
                }
                else
                {
                    // we need to redisplay the consent UI
                    result.ViewModel = await _mediator.Send
                    (
                        new BuildConsentViewModel
                        (
                            model.ReturnUrl,
                            model
                        )
                    );
                }

                return result;
            }
        }
    }
}