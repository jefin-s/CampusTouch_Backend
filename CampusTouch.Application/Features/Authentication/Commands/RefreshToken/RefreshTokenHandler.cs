

using CampusTouch.Application.Features.Authentication.DTOs;
using CampusTouch.Application.Interfaces;
using MediatR;

namespace CampusTouch.Application.Features.Authentication.Commands.RefreshToken
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, LoginResponseDTO>
    {
        private readonly IIdentityService _identityService;
        public RefreshTokenHandler(IIdentityService identityService)
        {
            _identityService = identityService;

        }
        public async Task<LoginResponseDTO> Handle(
       RefreshTokenCommand request,
       CancellationToken cancellationToken)
        {
            return await _identityService.RefreshTokenAsync(request.RefreshToken);
        }
    }
}
