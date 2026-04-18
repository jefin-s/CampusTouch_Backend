

//using CampusTouch.Application.Features.Authentication.DTOs;
//using CampusTouch.Application.Interfaces;
//using MediatR;

//namespace CampusTouch.Application.Features.Authentication.Commands.RefreshToken
//{
//    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, LoginResponseDTO>
//    {
//        private readonly IIdentityService _identityService;
//        public RefreshTokenHandler(IIdentityService identityService)
//        {
//            _identityService = identityService;

//        }
//        public async Task<LoginResponseDTO> Handle(
//       RefreshTokenCommand request,
//       CancellationToken cancellationToken)
//        {
//            return await _identityService.RefreshTokenAsync(request.RefreshToken);
//        }
//    }
//}
using CampusTouch.Application.Features.Authentication.DTOs;
using CampusTouch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CampusTouch.Application.Features.Authentication.Commands.RefreshToken
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, LoginResponseDTO>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<RefreshTokenHandler> _logger;

        public RefreshTokenHandler(
            IIdentityService identityService,
            ILogger<RefreshTokenHandler> logger)
        {
            _identityService = identityService;
            _logger = logger;
        }

        public async Task<LoginResponseDTO> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Refresh token attempt");

            try
            {
                var result = await _identityService.RefreshTokenAsync(request.RefreshToken);

                if (result == null)
                {
                    // ⚠ Invalid/expired token (normal case)
                    _logger.LogWarning("Invalid or expired refresh token attempt");
                    throw new UnauthorizedAccessException("Invalid refresh token");
                }

                _logger.LogInformation("Refresh token successful");

                return result;
            }
            catch (Exception ex)
            {
                // ❌ Unexpected failure
                _logger.LogError(ex, "Error during refresh token process");
                throw;
            }
        }
    }
}
