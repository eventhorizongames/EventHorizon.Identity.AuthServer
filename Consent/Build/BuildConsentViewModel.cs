using EventHorizon.Identity.AuthServer.Consent.Models;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Consent.Build
{
    public struct BuildConsentViewModel
        : IRequest<ConsentViewModel>
    {
        public string ReturnUrl { get; }
        public ConsentInputModel Model { get; }

        public BuildConsentViewModel(
            string returnUrl,
            ConsentInputModel model
        )
        {
            ReturnUrl = returnUrl;
            Model = model;
        }

        public BuildConsentViewModel(
            string returnUrl
        )
        {
            ReturnUrl = returnUrl;
            Model = default(
                ConsentViewModel
            );
        }
    }
}
