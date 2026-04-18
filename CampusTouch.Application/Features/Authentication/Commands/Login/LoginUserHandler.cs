using CampusTouch.Application.Features.Authentication.DTOs;
using CampusTouch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CampusTouch.Application.Features.Authentication.Commands.Login
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginResponseDTO>
    {
        private readonly IIdentityService _identityService;

        private readonly ILogger<LoginUserHandler> _logger;
        public LoginUserHandler(IIdentityService identityService, ILogger<LoginUserHandler> logger)
        {
            _identityService = identityService;
            _logger = logger;
        }


        public async Task<LoginResponseDTO> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var dto = request.dto;
            _logger.LogInformation("Attempting to log in user with email: {Email}", dto.Email);
            try
            {
                var result = await _identityService.LoginAsync(
                    dto.Email,
                    dto.Password
                );
                _logger.LogInformation("User {Email} logged in successfully", dto.Email);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while logging in user with email: {Email}", dto.Email);
                throw;
            }
        }
    }
}