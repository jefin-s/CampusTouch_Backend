using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.AssignSubject.Commands
{
    public class AssignSubjectCommand:IRequest<bool>
    {
        public int  StaffId { get; set; }
        
        public List<int> SubjectIds { get; set; }
    }
}
