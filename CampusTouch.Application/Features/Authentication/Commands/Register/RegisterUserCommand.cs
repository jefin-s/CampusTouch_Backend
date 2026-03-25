using CampusTouch.Application.Features.Authentication.DTOs;
using MediatR;

public record RegisterUserCommand(RegisterDTO NewRegistertion) : IRequest<string>;