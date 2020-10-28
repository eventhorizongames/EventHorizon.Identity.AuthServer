using System.Security.Claims;
using EventHorizon.Identity.AuthServer.Consent.Models;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Consent.Processing
{
    public struct ProcessPageConsentRequest
        : IRequest<ProcessConsentResult>
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
    }
}
