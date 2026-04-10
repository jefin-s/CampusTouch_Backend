using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Authentication.DTOs
{
    public class GoogleLoginResponseDTO
    {
        public string Token { get; set; }
        public string Email { get; set; }
    }
}
