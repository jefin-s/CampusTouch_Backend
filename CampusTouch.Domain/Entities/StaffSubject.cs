using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Domain.Entities
{
    public class StaffSubject
    {
        public int Id { get; set; }

        public int StaffId { get; set; }
        public int SubjectId { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }

    }
}
