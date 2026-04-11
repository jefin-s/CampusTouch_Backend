using CampusTouch.Application.Features.Attendence.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Attendence.Queries
{
    public class GetAttendenceByClassQuery:IRequest<IEnumerable<AttendenceViewDto>>
    {
        public int  ClassId { get; set; }
        public DateTime Date { get; set; }
    }
}
