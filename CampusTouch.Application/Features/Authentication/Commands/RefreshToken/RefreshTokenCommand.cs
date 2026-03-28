

using CampusTouch.Application.Features.Authentication.DTOs;
using MediatR;

namespace CampusTouch.Application.Features.Authentication.Commands.RefreshToken
{
    public record RefreshTokenCommand(string RefreshToken) :IRequest<LoginResponseDTO>;
    
}
