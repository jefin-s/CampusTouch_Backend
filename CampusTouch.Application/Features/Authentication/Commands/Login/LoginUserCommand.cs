using CampusTouch.Application.Features.Authentication.DTOs;
using MediatR;
    
public record LoginUserCommand(LoginDTO dto):IRequest<LoginResponseDTO>;