using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Domain.Entities
{
    public class AcademicProgram
    {
        public int Id { get; set; }

        public string Name { get; set; }          // BSc Computer Science
        public string Level { get; set; }         // UG / PG
        public int Duration { get; set; }         // Years (3 / 2)

        public int DepartmentId { get; set; }     // FK

        // Audit Fields (VERY IMPORTANT 🔥)
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
