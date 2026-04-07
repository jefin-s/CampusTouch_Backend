using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Staffs.Commands
{
    public class CreateStaffCommand:IRequest<bool>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public int DepartmentId { get; set; }
        public string Designation { get; set; }
    }
}
