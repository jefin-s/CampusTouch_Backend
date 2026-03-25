using CampusTouch.Application.Features.Authentication.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<string> RegisterAsync(
            string fullName,
            string email,
            string password,
            string phoneNumber
        );

        Task<LoginResponseDTO> LoginAsync(
          string email,
          string password
      );
    }
}
