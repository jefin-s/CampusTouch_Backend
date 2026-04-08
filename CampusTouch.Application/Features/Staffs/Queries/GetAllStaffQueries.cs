using CampusTouch.Application.Features.Staffs.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Staffs.Queries
{
    public  class GetAllStaffQueries:IRequest<IEnumerable<StaffDto>>
    {
    }
}
