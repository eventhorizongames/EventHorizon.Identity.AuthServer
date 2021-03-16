using System;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Clients.Services.Remove;
using EventHorizon.Identity.AuthServer.Models.Commands;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHorizon.Identity.AuthServer.Clients.Api
{
    [ApiController]
    [Route("api/clients/{clientId}")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ClientsRemoveController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClientsRemoveController(
            IMediator mediator
        )
        {
            _mediator = mediator;
        }

        [HttpDelete("remove")]
        public async Task<ActionResult<CommandResult<bool>>> Remove(
            string clientId
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new CommandResult<bool>(
                        "client_delete_request_not_valid"
                    )
                );
            }
            var result = await _mediator.Send(
                new RemoveClientCommand(
                    clientId
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
