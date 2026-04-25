using CampusTouch.Application.Common;
using CampusTouch.Application.Features.Authentication.DTOs;
using CampusTouch.Application.Features.Authentication.Commands.Register;
using CampusTouch.Application.Features.Authentication.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CampusTouch.Application.Features.Authentication.Commands.RefreshToken;

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


        //[HttpPost("login")]
        //public async Task<ActionResult> Login(LoginDTO dto)
        //{
        //    var result = await _mediator.Send(new LoginUserCommand(dto));

        //    return Ok(new ApiResponse<LoginResponseDTO>
        //    {
        //        Success = true,
        //        Message = "Login successful",
        //        Data = result
        //    });
        //}
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDTO dto)
        {
            var result = await _mediator.Send(new LoginUserCommand(dto));

            // 🔐 Store refresh token in HttpOnly cookie
            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // use true in production (HTTPS)
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new ApiResponse<LoginResponseDTO>
            {
                Success = true,
                Message = "Login successful",
                Data = new LoginResponseDTO
                {
                    Token=result.Token,
                    //RefreshToken=result.RefreshToken,
                    Email=result.Email, 
                    Roles=result.Roles
                    // ❌ DO NOT return refresh token
                }
            });
        }

        //[HttpPost("refresh")]

        //public async Task<ActionResult> Refresh([FromBody] RefreshTokenCommand request)
        //{
        //    var result = await _mediator.Send(
        //   new RefreshTokenCommand(request.RefreshToken));

        //    return Ok(new ApiResponse<LoginResponseDTO>
        //    {
        //        Success=true,
        //        Message="token refreshed successfully",
        //        Data=result
        //    } );
        //}
        [HttpPost("refresh")]
        public async Task<ActionResult> Refresh()
        {
            // 🔐 Get refresh token from cookie
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Refresh token missing",
                    Data = null
                });
            }

            var result = await _mediator.Send(new RefreshTokenCommand(refreshToken));

            // 🔄 (Optional but recommended) update cookie with new refresh token
            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new ApiResponse<LoginResponseDTO>
            {
                Success = true,
                Message = "Token refreshed successfully",
                Data = new LoginResponseDTO
                {
                    Token = result.Token,
                }
            });
        }

    }
}