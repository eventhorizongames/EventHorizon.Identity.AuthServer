using System.Linq;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Clients.Models;
using EventHorizon.Identity.AuthServer.Configuration;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Identity.AuthServer.Clients
{
    [Authorize("Identity Admin")]
    [Route("[controller]")]
    public partial class ClientController : Controller
    {
        readonly HistoryExtendedConfigurationDbContext _configurationDbContext;
        public ClientController(HistoryExtendedConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new ClientViewModel();
            foreach (var client in _configurationDbContext.Clients)
            {
                model.Clients.Add(client);
            }

            return View(model);
        }

        [HttpPost("{id}/Create")]
        public async Task<IActionResult> Create(ClientCreateModel model)
        {
            var entity = await _configurationDbContext.Clients.AddAsync(new Client()
            {
                ClientId = model.Id,
                ClientName = model.Name,
            });
            await _configurationDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index), "Client");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = _configurationDbContext.Clients
                .Include("AllowedScopes")
                .Include("AllowedGrantTypes")
                .Include("ClientSecrets")
                .Include("RedirectUris")
                .Include("PostLogoutRedirectUris")
                .Include("AllowedCorsOrigins")
                .Include("AllowedScopes")
                .FirstOrDefault(a => a.ClientId == id);
            return View(new ClientModel
            {
                Entity = entity,
                ApiResourceList = await _configurationDbContext.ApiResources.ToListAsync(),
                ClientId = id,

                AllowOfflineAccess = entity.AllowOfflineAccess,
                AllowAccessTokensViaBrowser = entity.AllowAccessTokensViaBrowser,
                RequireClientSecret = entity.RequireClientSecret,
            });
        }

        [HttpPost("{id}")]
        public IActionResult Edit(ClientModel model)
        {
            var entity = _configurationDbContext.Clients.FirstOrDefault(a => a.ClientId == model.ClientId);

            entity.ClientName = model.ClientName ?? entity.ClientName;
            entity.FrontChannelLogoutUri = model.FrontChannelLogoutUri ?? entity.FrontChannelLogoutUri;
            entity.AllowOfflineAccess = model.AllowOfflineAccess;
            entity.AllowAccessTokensViaBrowser = model.AllowAccessTokensViaBrowser;
            entity.RequireClientSecret = model.RequireClientSecret;

            _configurationDbContext.Clients.Update(entity);
            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Edit), "Client", new { id = model.ClientId });
        }

        [HttpPost("{id}/Delete")]
        public IActionResult Delete(string id)
        {
            _configurationDbContext.Clients.Remove(
                _configurationDbContext.Clients.FirstOrDefault(a => a.ClientId == id)
            );
            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Index), "Client");
        }
    }
}