using CampusTouch.Application.Common;
using CampusTouch.Application.Features.Authentication.DTOs;
using CampusTouch.Application.Features.Authentication.Commands.Register;
using CampusTouch.Application.Features.Authentication.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CampusTouch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDTO dto)
        {
            var result = await _mediator.Send(new RegisterUserCommand(dto));

            return StatusCode(201, new ApiResponse<string>
            {
                Success = true,
                Message = result,
                Data = null
            });
        }

        
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDTO dto)
        {
            var result = await _mediator.Send(new LoginUserCommand(dto));

            return Ok(new ApiResponse<LoginResponseDTO>
            {
                Success = true,
                Message = "Login successful",
                Data = result
            });
        }
    }
}