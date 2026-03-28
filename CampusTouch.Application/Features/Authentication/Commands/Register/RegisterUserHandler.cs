using CampusTouch.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Authentication.Commands.Register;

public  class RegisterUserHandler:IRequestHandler<RegisterUserCommand,string>
{

    private readonly IIdentityService _identityService;
    public RegisterUserHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<string> Handle(RegisterUserCommand  request,CancellationToken cancellation)
    {
        var dto = request.NewRegistertion;
        return await _identityService.RegisterAsync(
            dto.FullName,
            dto.Email,
            dto.Password,
            dto.PhoneNumber,
            dto.Username
        );

    }
}
