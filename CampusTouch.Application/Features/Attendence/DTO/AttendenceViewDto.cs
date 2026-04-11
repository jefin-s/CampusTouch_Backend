using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Attendence.DTO
{
    public class AttendenceViewDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string Status { get; set; }
    }
}
