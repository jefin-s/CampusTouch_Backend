using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Domain.Entities
{
    public class Classes
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public int CourseId { get; set; }
        public int Year { get; set; }
        public int Semester { get; set; }
        public  bool IsActive { get; set; } 
    }
}
