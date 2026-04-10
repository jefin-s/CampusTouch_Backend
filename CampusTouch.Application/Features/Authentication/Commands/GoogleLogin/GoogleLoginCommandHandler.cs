using CampusTouch.Application.Features.Authentication.DTOs;
using CampusTouch.Application.Interfaces;
using MediatR;
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
        public GoogleLoginCommandHandler(IGoogleAuthService googleAuthService, IJWTService jWTService)
        {
            _googleAuthService = googleAuthService;
            _jwtService = jWTService;

        }
        public async Task<GoogleLoginResponseDTO> Handle(
      GoogleLoginCommand request,
      CancellationToken cancellationToken)
        {
            var user = await _googleAuthService.AuthenticateAsync();

            var token = _jwtService.GenerateToken(user.Id, user.Email, user.Roles);

            return new GoogleLoginResponseDTO
            {
                Token = token,
                Email = user.Email
            };
        }
    }
}
