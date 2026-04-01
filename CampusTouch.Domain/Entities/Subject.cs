using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Domain.Entities
{
    public class Subject
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public int Credits { get; set; }

        public int SemesterId { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public string  updatedby { get; set; }
        public string creatdby { get; set; }
        public DateTime deletedAt { get; set; }

        public string deletedby { get; set; }

    }
}
