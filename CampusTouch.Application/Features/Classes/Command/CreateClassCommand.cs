using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Classes.Command
{
    public  class CreateClassCommand:IRequest<int>
    {
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public int CourseId { get; set; }
        public int Year { get; set; }
        public int Semester { get; set; }
    }
}
