using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Features.Authentication.DTOs;
using CampusTouch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Authentication.Commands.GoogleLogin
{
    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, GoogleLoginResponseDTO>
    {
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IJWTService _jwtService;
        private readonly ILogger<GoogleLoginCommandHandler> _logger;
        public GoogleLoginCommandHandler(IGoogleAuthService googleAuthService, IJWTService jWTService,ILogger<GoogleLoginCommandHandler> logger)
        {
            _googleAuthService = googleAuthService;
            _jwtService = jWTService;
            _logger = logger;

        }
        public async Task<GoogleLoginResponseDTO> Handle(
     GoogleLoginCommand request,
     CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Google login attempt started");

                var user = await _googleAuthService.AuthenticateAsync();

                if (user == null)
                {
                    _logger.LogWarning("Google authentication failed");
                    throw new NotFoundException("Google authentication failed");
                }

                if (user.Roles == null || !user.Roles.Any())
                {
                    _logger.LogWarning("User {Email} has no roles assigned", user.Email);
                    throw new NotFoundException("User has no roles assigned");
                }

                var token = _jwtService.GenerateToken(user.Id, user.Email, user.Roles);

                _logger.LogInformation("User {Email} logged in via Google successfully", user.Email);

                return new GoogleLoginResponseDTO
                {
                    Token = token,
                    Email = user.Email
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Google login");
                throw;
            }
        }
    }
}
