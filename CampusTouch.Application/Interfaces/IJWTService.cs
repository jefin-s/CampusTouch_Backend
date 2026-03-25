using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Interfaces
{
    public interface IJWTService
    {

        string GenerateToken(string userId, string email, IList<string> roles);
    }
}
