using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Consent.Models;
using IdentityServer4.Models;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Consent.Create
{
    public class CreateConsentViewModelHandler
        : IRequestHandler<CreateConsentViewModel, ConsentViewModel>
    {
        public Task<ConsentViewModel> Handle(
            CreateConsentViewModel request,
            CancellationToken cancellationToken
        )
        {
            var result = new ConsentViewModel
            {
                Description = request.Model?.Description ?? string.Empty,
                RememberConsent = request.Model?.RememberConsent ?? true,
                ScopesConsented = request.Model?.ScopesConsented ?? Enumerable.Empty<string>(),

                ReturnUrl = request.ReturnUrl,

                ClientName = request.Client.ClientName ?? request.Client.ClientId,
                ClientUrl = request.Client.ClientUri,
                ClientLogoUrl = request.Client.LogoUri,
                AllowRememberConsent = request.Client.AllowRememberConsent
            };

            result.IdentityScopes = request.Resources.IdentityResources.Select(
                x => CreateScopeViewModel(
                    x,
                    result.ScopesConsented.Contains(x.Name)
                        || request.Model == null
                )
            ).ToArray();

            result.ResourceScopes = request.Resources.ApiScopes.Select(
                x => CreateScopeViewModel(
                    x,
                    result.ScopesConsented.Contains(
                        x.Name
                    ) || request.Model == null
                )
            ).ToArray();

            if (ConsentOptions.EnableOfflineAccess && request.Resources.OfflineAccess)
            {
                result.ResourceScopes = result.ResourceScopes.Union(
                    new ScopeViewModel[]
                    {
                        GetOfflineAccessScope(
                            result.ScopesConsented.Contains(
                                IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess
                            ) || request.Model == null
                        )
                    }
                );
            }

            return Task.FromResult(
                result
            );
        }

        private ScopeViewModel CreateScopeViewModel(
            IdentityResource identity,
            bool check
        )
        {
            return new ScopeViewModel
            {
                Name = identity.Name,
                DisplayName = identity.DisplayName,
                Description = identity.Description,
                Emphasize = identity.Emphasize,
                Required = identity.Required,
                Checked = check || identity.Required
            };
        }

        public ScopeViewModel CreateScopeViewModel(
            ApiScope scope,
            bool check
        )
        {
            return new ScopeViewModel
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Description = scope.Description,
                Emphasize = scope.Emphasize,
                Required = scope.Required,
                Checked = check || scope.Required
            };
        }

        private ScopeViewModel GetOfflineAccessScope(
            bool check
        )
        {
            return new ScopeViewModel
            {
                Name = IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess,
                DisplayName = ConsentOptions.OfflineAccessDisplayName,
                Description = ConsentOptions.OfflineAccessDescription,
                Emphasize = true,
                Checked = check
            };
        }
    }
}
