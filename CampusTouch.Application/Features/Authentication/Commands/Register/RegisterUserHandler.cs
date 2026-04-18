using CampusTouch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Authentication.Commands.Register;

public  class RegisterUserHandler:IRequestHandler<RegisterUserCommand,string>
{

    private readonly IIdentityService _identityService;
    private readonly ILogger<RegisterUserHandler> _logger;
    public RegisterUserHandler(IIdentityService identityService,ILogger<RegisterUserHandler> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<string> Handle(RegisterUserCommand  request,CancellationToken cancellation)
    {
        var dto = request.NewRegistertion;
        _logger.LogInformation("User registration attempt for {Email}", dto.Email);
        try
        {

        var result= await _identityService.RegisterAsync(
            dto.FullName,
            dto.Email,
            dto.Password,
            dto.PhoneNumber,
            dto.Username,
            "Applicant"
        );
            _logger.LogInformation("User registration successful for {Email}", dto.Email);
            return result;
        }
        catch
            (Exception ex)
        {
            _logger.LogError(ex, "User registration failed for {Email}", dto.Email);
            throw;
        }

    }
}
