using CampusTouch.Application.Features.Authentication.DTOs;
using CampusTouch.Application.Interfaces;
using MediatR;

namespace CampusTouch.Application.Features.Authentication.Commands.Login
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginResponseDTO>
    {
        private readonly IIdentityService _identityService;

        public LoginUserHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

       
        public async Task<LoginResponseDTO> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.LoginAsync(
                request.dto.Email,     
                request.dto.Password   
            );
        }
    }
}