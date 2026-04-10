using CampusTouch.Application.Features.Authentication.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Interfaces
{
    public  interface IGoogleAuthService
    {
        Task<GoogleUserDto> AuthenticateAsync();
    }
}
