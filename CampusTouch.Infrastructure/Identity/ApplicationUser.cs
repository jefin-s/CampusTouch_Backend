

using Microsoft.AspNetCore.Identity;

namespace CampusTouch.Infrastructure.Identity
{
    public  class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; }    
    }
}
