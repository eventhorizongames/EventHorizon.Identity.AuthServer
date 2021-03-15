using System;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Clients.Models;
using EventHorizon.Identity.AuthServer.Clients.Services.Create;
using EventHorizon.Identity.AuthServer.Models.Commands;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHorizon.Identity.AuthServer.Clients.Api
{
    [ApiController]
    [Route("api/clients")]
    [Authorize("Identity Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ClientsCreateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClientsCreateController(
            IMediator mediator
        )
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<ActionResult<CommandResult<ClientModel>>> Create(
            ClientCreateModel client
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new CommandResult<ClientModel>(
                        "client_is_invalid"
                    )
                );
            }
            var result = await _mediator.Send(
                new CreateClientCommand(
                    client
                )
            );
            if (!result.Success)
            {
                return BadRequest(
                    result
                );
            }

            return result;
        }
    }
}
