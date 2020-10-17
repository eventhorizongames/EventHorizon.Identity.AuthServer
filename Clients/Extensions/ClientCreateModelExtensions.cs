using EventHorizon.Identity.AuthServer.Clients.Models;
using IdentityServer4.EntityFramework.Entities;

namespace EventHorizon.Identity.AuthServer.Clients.Extensions
{
    public static class ClientCreateModelExtensions
    {
        public static Client ToEntity(
            this ClientCreateModel client
        )
        {
            return new Client
            {
                ClientId = client.Id,
                ClientName = client.Name,

                AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser,
                RequireClientSecret = client.RequireClientSecret,
            };
        }
    }
}