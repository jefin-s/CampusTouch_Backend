using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Identity;
using CampusTouch.Infrastructure.Persistance.Identity;
using CampusTouch.Application.Features.Authentication.Commands.GoogleLogin;

namespace CampusTouch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalAuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ExternalAuthController(
            IMediator mediator,
            SignInManager<ApplicationUser> signInManager)
        {
            _mediator = mediator;
            _signInManager = signInManager;
        }

        // 🔹 Start Google Login
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse", "ExternalAuth");

            var properties = _signInManager
                .ConfigureExternalAuthenticationProperties("Google", redirectUrl);

            return Challenge(properties, "Google");
        }

        // 🔹 Handle Google Callback
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await _mediator.Send(new GoogleLoginCommand());

            return Ok(result);
        }
    }
}

