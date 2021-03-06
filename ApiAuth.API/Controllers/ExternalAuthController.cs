﻿using ApiAuth.API.DtoS;
using ApiAuth.API.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ApiAuth.API.Infrastructure.Abstraction.Commands;

namespace ApiAuth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalAuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExternalAuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetExternalLoginProviders()
        {
            var result = await _mediator.Send(new GetExternalLoginProvidersRequest());
            return Ok(new ResponseViewModel(result));
        }

        [HttpPost("facebook")]
        public async Task<IActionResult> Facebook([FromBody]FacebookAuthDto model)
        {
            var result = await _mediator.Send(new FacebookExternalLoginRequest(model.AccessToken));
            return Ok(new ResponseViewModel(result));
        }

        [HttpPost("google")]
        public async Task<IActionResult> Google([FromBody]GoogleAuthDto model)
        {
            var result = await _mediator.Send(new GoogleExternalLoginRequest(model.AccessToken, model.Code));
            return Ok(new ResponseViewModel(result));
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteProvider(string loginProvider, string providerKey)
        {
            await _mediator.Publish(new DeleteProviderEvent(User.Identity.Name, providerKey, loginProvider));
            return Ok(new ResponseViewModel("The external login was removed."));
        }
    }
}