using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.AssignSubject.Queries
{
    public  class GetSubjectByStaffQueries:IRequest<List<int>>
    {
        public int StaffId { get; set; }
    }
}
