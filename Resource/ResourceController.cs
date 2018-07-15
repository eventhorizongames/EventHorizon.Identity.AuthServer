using System.Linq;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Clients.Models;
using EventHorizon.Identity.AuthServer.Configuration;
using EventHorizon.Identity.AuthServer.Resource.Models;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.EntityFramework.Mappers;
using EventHorizon.Identity.AuthServer.Clients;

namespace EventHorizon.Identity.AuthServer.Resource
{
    [Authorize("Identity Admin")]
    [Route("[controller]")]
    public partial class ResourceController : Controller
    {
        readonly HistoryExtendedConfigurationDbContext _configurationDbContext;
        public ResourceController(HistoryExtendedConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_configurationDbContext.ApiResources.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult Edit(string id)
        {
            var entity = _configurationDbContext.ApiResources
                .Include("Scopes")
                .Include("UserClaims")
                .FirstOrDefault(a => a.Name == id);
            return View(new ResourceModel
            {
                Entity = entity,
                Name = id,
            });
        }

        [HttpPost("{id}")]
        public IActionResult Edit(ResourceModel model)
        {
            var entity = _configurationDbContext.ApiResources.FirstOrDefault(a => a.Name == model.Name);

            entity.DisplayName = model.DisplayName ?? entity.DisplayName;
            entity.Description = model.Description ?? entity.Description;

            _configurationDbContext.ApiResources.Update(entity);
            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Edit), "Resource", new { id = model.Name });
        }

        [HttpGet("Create")]
        public IActionResult Create([FromQuery]string returnClient)
        {
            return View(new ResourceCreateModel()
            {
                ReturnClient = returnClient,
            });
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm]ResourceCreateModel model)
        {
            var entity = await _configurationDbContext.ApiResources.AddAsync(new ApiResource(
                model.Name,
                model.DisplayName
            ).ToEntity());
            await _configurationDbContext.SaveChangesAsync();

            if (model.ReturnClient != null)
            {
                return RedirectToAction(nameof(ClientController.Edit), "Client", new { Id = model.ReturnClient });
            }
            return RedirectToAction(nameof(Index), "Resource");
        }

        [HttpPost("{id}/Delete")]
        public IActionResult Delete(string id)
        {
            _configurationDbContext.ApiResources.Remove(
                _configurationDbContext.ApiResources.FirstOrDefault(a => a.Name == id)
            );
            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Index), "Resource");
        }
    }
}