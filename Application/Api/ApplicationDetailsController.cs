using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace EventHorizon.Identity.AuthServer.Application.Api
{
    [ApiController]
    [Route("api/application")]
    public class ApplicationDetailsController
        : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ApplicationDetailsController(
            IConfiguration configuration
        )
        {
            this._configuration = configuration;
        }

        [HttpGet("details")]
        public ActionResult<ApplicationDetailsModel> Details()
        {
            return new ApplicationDetailsModel
            {
                ApplicationVersion = _configuration["APPLICATION_VERSION"],
            };
        }

        public class ApplicationDetailsModel
        {
            public string ApplicationVersion { get; set; }
        }
    }
}
