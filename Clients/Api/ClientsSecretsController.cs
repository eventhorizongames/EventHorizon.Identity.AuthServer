using System;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Clients.Models;
using EventHorizon.Identity.AuthServer.Clients.Services.Add;
using EventHorizon.Identity.AuthServer.Clients.Services.Remove;
using EventHorizon.Identity.AuthServer.Models.Commands;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHorizon.Identity.AuthServer.Clients.Api
{
    [ApiController]
    [Route("api/clients/{clientId}/secrets")]
    [Authorize("Identity Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ClientsSecretsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClientsSecretsController(
            IMediator mediator
        )
        {
            _mediator = mediator;
        }

        [HttpPost("add")]
        public async Task<ActionResult<CommandResult<ClientModel>>> Add(
            string clientId,
            AddSecretModel secret
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new CommandResult<ClientModel>(
                        "secret_is_invalid"
                    )
                );
            }
            var result = await _mediator.Send(
                new AddClientSecretCommand(
                    clientId,
                    secret
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

        [HttpDelete("remove")]
        public async Task<ActionResult<CommandResult<bool>>> Add(
            string clientId,
            [FromQuery]
            string description
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new CommandResult<bool>(
                        "secret_is_invalid"
                    )
                );
            }
            var result = await _mediator.Send(
                new RemoveClientSecretCommand(
                    clientId,
                    description
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
