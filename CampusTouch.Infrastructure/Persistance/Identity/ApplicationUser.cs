using Microsoft.AspNetCore.Identity;

namespace CampusTouch.Infrastructure.Persistance.Identity
{
    public  class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; }    
    }
}
