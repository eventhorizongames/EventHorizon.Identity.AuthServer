using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Clients.Services.Query;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHorizon.Identity.AuthServer.Clients.Api
{
    [ApiController]
    [Route("api/clients/{clientId}")]
    [Authorize("SystemAdmin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ClientsCheckController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClientsCheckController(
            IMediator mediator
        )
        {
            _mediator = mediator;
        }

        [HttpGet("check")]
        public async Task<ActionResult<bool>> Create(
            string clientId
        )
        {
            var result = await _mediator.Send(
                new QueryForClientById(
                    clientId
                )
            );

            return result.Success;
        }
    }
}
