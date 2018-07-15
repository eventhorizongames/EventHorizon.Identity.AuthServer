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

        [HttpGet("Edit/{name}")]
        public IActionResult Edit(string name)
        {
            var entity = _configurationDbContext.ApiResources
                .Include("Scopes")
                .Include("UserClaims")
                .FirstOrDefault(a => a.Name == name);
            return View(new ResourceModel
            {
                Entity = entity,
                Name = name,
            });
        }

        [HttpPost("Edit/{name}")]
        public IActionResult Edit(ResourceModel model)
        {
            var entity = _configurationDbContext.ApiResources.FirstOrDefault(a => a.Name == model.Name);

            entity.DisplayName = model.DisplayName ?? entity.DisplayName;
            entity.Description = model.Description ?? entity.Description;

            _configurationDbContext.ApiResources.Update(entity);
            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Edit), "Resource", new { name = model.Name });
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

        [HttpDelete("{name}")]
        public IActionResult Delete(string name)
        {
            _configurationDbContext.ApiResources.Remove(
                _configurationDbContext.ApiResources.FirstOrDefault(a => a.Name == name)
            );
            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Index), "Resource");
        }
    }
}