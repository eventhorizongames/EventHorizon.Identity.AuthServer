using EventHorizon.Identity.AuthServer.Consent.Models;
using IdentityServer4.Models;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Consent.Create
{
    public struct CreateConsentViewModel 
        : IRequest<ConsentViewModel>
    {
        public ConsentInputModel Model { get; set; }
        public string ReturnUrl { get; set; }
        public AuthorizationRequest AuthorizationRequest { get; set; }
        public Client Client { get; set; }
        public Resources Resources { get; set; }

        public CreateConsentViewModel(
            ConsentInputModel model,
            string returnUrl,
            AuthorizationRequest authorizationRequest,
            Client client,
            Resources resources
        )
        {
            Model = model;
            ReturnUrl = returnUrl;
            AuthorizationRequest = authorizationRequest;
            Client = client;
            Resources = resources;
        }
    }
}
