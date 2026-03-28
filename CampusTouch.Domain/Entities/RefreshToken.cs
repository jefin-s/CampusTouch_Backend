using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Domain.Entities
{
    public  class RefreshToken
    {
        public Guid Id { get; set; }

        public string Token { get; set; } = default!;

        public DateTime Expires { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public bool Revoked { get; set; } = false;

        public string UserId { get; set; } = default!;
       
    }
}
